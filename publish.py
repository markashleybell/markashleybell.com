# This Python file uses the following encoding: utf-8
import argparse, markdown, datetime, codecs, re, os, glob, time, re, ConfigParser, PyRSS2Gen
import paramiko, base64, getpass, socket, sys, traceback # Only needed for uploads
from string import Template

# Parse headers (date and title) from post file content
def get_post_data(content):
    # Fields are empty by default
    metadata = { 'title': None, 'date': None, 'content': None, 'abstract': None }
    # Try and get the title
    titlere = re.compile("(^Title: (.*)[\r\n]+)", re.IGNORECASE | re.MULTILINE)
    match = titlere.search(content)
    if match:
        metadata['title'] = match.group(2).strip()
    # Try and get the date
    datere = re.compile("(^Date: (.*)[\r\n]+)", re.IGNORECASE | re.MULTILINE)
    match = datere.search(content)
    if match:
        metadata['date'] = datetime.datetime.strptime(match.group(2).strip(), '%Y-%m-%d %H:%M')
    # Try and get the abstract
    abstractre = re.compile("(^Abstract: (.*)[\r\n]+)", re.IGNORECASE | re.MULTILINE)
    match = abstractre.search(content)
    if match:
        metadata['abstract'] = match.group(2).strip()
    # Remove the title and date header lines if they were present
    content_no_metadata = titlere.sub('', content)
    content_no_metadata = datere.sub('', content_no_metadata)
    metadata['content'] = abstractre.sub('', content_no_metadata)
    return metadata

# Set up command-line arguments
parser = argparse.ArgumentParser(description='Generate a blog from Markdown files and HTML templates')
parser.add_argument('-p', '--publish', help='publish to remote server', action='store_true')
args = parser.parse_args()

# Load config
config = ConfigParser.RawConfigParser()
config.read('config.cfg')

minify = '.min' if config.get('Debug', 'minify') is True else ''

# Load the master page template
master_template_file = codecs.open('templates/master.template', 'r', 'utf-8')
master_template = Template(master_template_file.read())

# Load the template HTML for individual posts
post_template_file = codecs.open('templates/post.template', 'r', 'utf-8')
post_template = Template(post_template_file.read())

# Load the template HTML for comments
comment_template_file = codecs.open('templates/disqus.template', 'r', 'utf-8')
comment_template = Template(comment_template_file.read())

# Get the path of this script and the path of the parent (the destination for generated HTML)
currentpath = os.path.dirname(os.path.abspath(__file__))
web_root = currentpath + '/public'

# Build a sortable list of file information
file_list = []

# Loop through all Markdown files in the current folder
for folder in glob.glob(currentpath):
    # Select the type of file
    for f in glob.glob(folder + '/posts/*.md'):
        # Get the filename portion of the path
        markdown_file = os.path.split(f)[1]
        # Open the Markdown file and get the first line (heading)
        md = codecs.open(f, 'r', 'utf-8')
        post_data = get_post_data(md.read())
        # Replace the .md extension with .html
        html_file = re.sub(r"(?si)^(.*\.)(md)$", r"\1html", markdown_file)
        # Create a list of tuples containing post data, ready for sorting by date
        file_tuple = post_data['date'], post_data['title'], post_data['content'], markdown_file, html_file, post_data['abstract']
        file_list.append(file_tuple)
 
# Sort the file list by post date descending 
file_list.sort()
file_list.reverse()

# Just delete all existing HTML files to avoid orphans
for f in glob.glob(web_root + '/*.html'):
   os.unlink (f)

# Build a list of links to individual posts
nav_items = []

# Loop through all the files in the list and create a nav list item for each
# We create the nav before the post HTML because it is included in every page
for inputfile in file_list:
    # Add the item to nav (we are ordered by created date descending, so we're adding in the correct order)
    nav_item = u'<li><a href="' + inputfile[4] + '">' + inputfile[1] + '</a></li>'
    nav_items.append(nav_item)

nav = u'<ul>' + ''.join(nav_items) + '</ul>'

# Collect the first n posts to display on the home page
homepage = []
homepage_post_count = 5
rss = []
rss_post_count = 5

for inputfile in file_list:
    # Populate the post template HTML
    # post_date = inputfile[0].strftime('%A %d %B, %Y %H:%M') if inputfile[0] is not None else 'COULD NOT PARSE DATE'
    # post_date = inputfile[0].strftime('%d %b, %Y, %H:%M') if inputfile[0] is not None else 'COULD NOT PARSE DATE'
    post_date = inputfile[0].strftime('%d %b, %Y') if inputfile[0] is not None else 'COULD NOT PARSE DATE'
    post = post_template.substitute(date = post_date, heading = inputfile[1], permalink = inputfile[4], body = markdown.markdown(inputfile[2], extensions=['extra', 'codehilite']))
    # If there are less than 5 posts in the homepage list, add this one
    if len(homepage) < homepage_post_count: 
        if inputfile[5] is not None:
            short_post = post_template.substitute(date = post_date, heading = inputfile[1], permalink = inputfile[4], body = markdown.markdown(inputfile[5] + ' <a class="more-link" href="' + inputfile[4] + '">&rarr;</a>', extensions=['extra']))
            homepage.append(short_post)
        else:
            homepage.append(post)
    # Add the raw post details to the RSS feed
    if len(rss) < rss_post_count: 
        rss.append(inputfile)
    # Populate the master template with the populated post HTML
    output = master_template.substitute(content = post, nav = nav, title = inputfile[1] + ' - ', minify = minify, comments = comment_template.substitute())
    # Write out the processed HTML file for this post
    o = codecs.open(web_root + '/' + inputfile[4], 'w', 'utf-8')
    o.write(output)
    o.close()

# Create the index page, passing in the joined HTML for the homepage posts
output = master_template.substitute(content = '\r\n'.join(homepage), nav = nav, title = '', minify = minify, comments = '')
o = codecs.open(web_root + '/index.html', 'w', 'utf-8')
o.write(output)
o.close()

hostname = config.get('Site', 'hostname')

def map_rss_item(x):
    return PyRSS2Gen.RSSItem(
            title = x[1],
            link = "http://" + hostname + "/" + x[4],
            description = markdown.markdown(x[2], extensions=['extra']),
            guid = PyRSS2Gen.Guid("http://" + hostname + "/" + x[4]),
            pubDate = x[0]
        )

rss_feed = PyRSS2Gen.RSS2(
        title = "markashleybell.com",
        link = "http://" + hostname,
        description = "The latest articles from " + hostname,
        lastBuildDate = datetime.datetime.now(),
        items = map(map_rss_item, rss)
    )

rss_feed.write_xml(codecs.open(web_root + '/rss.xml', 'w', 'utf-8'))

print 'File generation complete'

# Load config for upload
secure = config.getboolean('FTP', 'secure')
hostname = config.get('FTP', 'hostname')
username = config.get('FTP', 'username')
password = config.get('FTP', 'password')
remotepath = config.get('FTP', 'remotepath')

if args.publish and secure:
    # Set up log
    paramiko.util.log_to_file('sftp.log')

    # Connect to the server
    ssh = paramiko.SSHClient()
    ssh.set_missing_host_key_policy(paramiko.AutoAddPolicy())
    ssh.connect(hostname, username=username, password=password)
    sftp = ssh.open_sftp()

    # Check if the CSS folder exists, and if not create it
    try:
        s = sftp.stat(remotepath + '/css')
    except IOError as e:
        print 'Creating css folder'
        sftp.mkdir(remotepath + '/css')

    # Upload the CSS files
    sftp.put(web_root + '/css/bootstrap.css', remotepath + '/css/bootstrap.css')
    sftp.put(web_root + '/css/github.css', remotepath + '/css/github.css')
    sftp.put(web_root + '/css/styles.css', remotepath + '/css/styles.css')
    sftp.put(web_root + '/css/styles.min.css', remotepath + '/css/styles.min.css')

    # Check if the image folder exists, and if not create it
    try:
        s = sftp.stat(remotepath + '/img')
    except IOError as e:
        print 'Creating img folder'
        sftp.mkdir(remotepath + '/img')
        sftp.mkdir(remotepath + '/img/post')

    # Upload the favicon
    sftp.put(web_root + '/img/favicon.ico', remotepath + '/img/favicon.ico')

    # Check if the script folder exists, and if not create it
    try:
        s = sftp.stat(remotepath + '/js')
    except IOError as e:
        print 'Creating js folder'
        sftp.mkdir(remotepath + '/js')

    # Upload the script
    sftp.put(web_root + '/js/rainbow-custom.min.js', remotepath + '/js/rainbow-custom.min.js')
    sftp.put(web_root + '/js/main.js', remotepath + '/js/main.js')

    # Upload all post images
    for f in glob.glob(web_root + '/img/post/*.jpg'):
        sftp.put(f, remotepath + '/img/post/' + os.path.split(f)[1])
        print '/img/post/' + os.path.split(f)[1] + ' -> /img/post/' + os.path.split(f)[1]

    # Upload all the HTML files
    for f in glob.glob(web_root + '/*.html'):
        sftp.put(f, remotepath + '/' + os.path.split(f)[1])
        print '/' + os.path.split(f)[1] + ' -> /' + os.path.split(f)[1]

    # Upload the RSS feed XML
    sftp.put(web_root + '/rss.xml', remotepath + '/rss.xml')

    print 'Publish complete'