# This Python file uses the following encoding: utf-8
import argparse, markdown, datetime, codecs, re, os, glob, time, re, ConfigParser, PyRSS2Gen
import paramiko, base64, getpass, socket, sys, traceback # Only needed for uploads
from string import Template

# Parse headers (date and title) from post file content
def get_post_data(content):
    # Fields are empty by default
    metadata = { 'title': None, 'date': None, 'content': None, 'abstract': None, 'pagetype': None }
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
    # Try and get the type
    pagetypere = re.compile("(^PageType: (.*)[\r\n]+)", re.IGNORECASE | re.MULTILINE)
    match = pagetypere.search(content)
    if match:
        metadata['pagetype'] = match.group(2).strip()
    # Remove the header lines if they were present
    content_no_metadata = titlere.sub('', content)
    content_no_metadata = datere.sub('', content_no_metadata)
    content_no_metadata = abstractre.sub('', content_no_metadata)
    metadata['content'] = pagetypere.sub('', content_no_metadata)
    return metadata

# Set up command-line arguments
parser = argparse.ArgumentParser(description='Generate a blog from Markdown files and HTML templates')
parser.add_argument('-p', '--publish', help='publish to remote server', action='store_true')
args = parser.parse_args()

# Load config
config = ConfigParser.RawConfigParser()
config.read('config.cfg')

# Determine whether to use minified versions of scripts and CSS
minify = '.min' if config.getboolean('Debug', 'minify') else ''

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
        file_tuple = post_data['date'], post_data['title'], post_data['content'], markdown_file, html_file, post_data['abstract'], post_data['pagetype']
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
    if inputfile[6] != 'static':
        # Add the item to nav (we are ordered by created date descending, so we're adding in the correct order)
        nav_item = u'<li><a href="' + inputfile[4] + '">' + inputfile[1] + '</a></li>'
        nav_items.append(nav_item)

nav = u'<ul>' + ''.join(nav_items) + '</ul>'

# Collect the first n posts to display on the home page
homepage = []
homepage_post_count = 5
rss = []
rss_post_count = 10

for inputfile in file_list:
    # Populate the post template HTML
    # post_date = inputfile[0].strftime('%A %d %B, %Y %H:%M') if inputfile[0] is not None else 'COULD NOT PARSE DATE'
    # post_date = inputfile[0].strftime('%d %b, %Y, %H:%M') if inputfile[0] is not None else 'COULD NOT PARSE DATE'
    post_date = inputfile[0].strftime('%d %b, %Y') if inputfile[0] is not None else 'COULD NOT PARSE DATE'
    post = post_template.substitute(date = post_date, heading = inputfile[1], permalink = inputfile[4], body = markdown.markdown(inputfile[2], extensions=['extra', 'codehilite']))
    # If there are less than 5 posts in the homepage list, add this one
    if len(homepage) < homepage_post_count and inputfile[6] != 'static':
        if inputfile[5] is not None:
            short_post = post_template.substitute(date = post_date, heading = inputfile[1], permalink = inputfile[4], body = markdown.markdown(inputfile[5] + ' <a class="more-link" href="' + inputfile[4] + '">&rarr;</a>', extensions=['extra']))
            homepage.append(short_post)
        else:
            homepage.append(post)
    # Add the raw post details to the RSS feed
    if len(rss) < rss_post_count and inputfile[6] != 'static': 
        rss.append(inputfile)
    # Populate the master template with the populated post HTML
    comments = '' if inputfile[6] == 'static' else comment_template.substitute()
    output = master_template.substitute(content = post, nav = nav, title = inputfile[1] + ' - Mark Ashley Bell', minify = minify, comments = comments)
    # Write out the processed HTML file for this post
    o = codecs.open(web_root + '/' + inputfile[4], 'w', 'utf-8')
    o.write(output)
    o.close()

# Create the index page, passing in the joined HTML for the homepage posts
output = master_template.substitute(content = '\r\n'.join(homepage), nav = nav, title = 'Mark Ashley Bell, Freelance Web Designer/Developer - C# ASP.NET, jQuery, JavaScript and Python web development', minify = minify, comments = '')
o = codecs.open(web_root + '/index.html', 'w', 'utf-8')
o.write(output)
o.close()

hostname = config.get('Site', 'hostname')

def map_rss_item(x):
    # Replace any relative urls with absolute URLs
    body = re.sub(r"(/img[a-z0-9\/\-\_\.]+)", r"http://markashleybell.com\1", x[2])

    # TODO: Remove hard-coded hostname above - when the variable is substituted into 
    #       the regex as below, the RSS output is truncated for some reason?
    # body = re.sub(r"(/img[a-z0-9\/\-\_\.]+)", r"http://" + hostname + "\1", x[2])

    return PyRSS2Gen.RSSItem(
            title = x[1],
            link = "http://" + hostname + "/" + x[4],
            description = markdown.markdown(body, extensions=['extra']),
            guid = PyRSS2Gen.Guid("http://" + hostname + "/" + x[4]),
            pubDate = x[0]
        )

rss_feed = PyRSS2Gen.RSS2(
        title = hostname,
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

    # Upload .htaccess
    sftp.put(web_root + '/.htaccess', remotepath + '/.htaccess')

    # Check if the CSS folder exists, and if not create it
    try:
        s = sftp.stat(remotepath + '/css')
    except IOError as e:
        print 'Creating css folder'
        sftp.mkdir(remotepath + '/css')

    # Upload the CSS files
    for f in glob.glob(web_root + '/css/*.css'):
        sftp.put(web_root + '/css/' + os.path.basename(f), remotepath + '/css/' + os.path.basename(f))

    # Check if the image folder exists, and if not create it
    try:
        s = sftp.stat(remotepath + '/img')
    except IOError as e:
        print 'Creating img folder'
        sftp.mkdir(remotepath + '/img')
        sftp.mkdir(remotepath + '/img/post')

    # Upload the favicon and logo
    sftp.put(web_root + '/img/favicon.ico', remotepath + '/img/favicon.ico')
    sftp.put(web_root + '/img/logo.png', remotepath + '/img/logo.png')

    # Check if the script folder exists, and if not create it
    try:
        s = sftp.stat(remotepath + '/js')
    except IOError as e:
        print 'Creating js folder'
        sftp.mkdir(remotepath + '/js')

    # Upload the script
    for f in glob.glob(web_root + '/js/*.js'):
        sftp.put(web_root + '/js/' + os.path.basename(f), remotepath + '/js/' + os.path.basename(f))

    # Upload all post images
    for f in glob.glob(web_root + '/img/post/*.jpg'):
        sftp.put(f, remotepath + '/img/post/' + os.path.basename(f))
        print '/img/post/' + os.path.basename(f) + ' -> /img/post/' + os.path.basename(f)

    for f in glob.glob(web_root + '/img/post/*.gif'):
        sftp.put(f, remotepath + '/img/post/' + os.path.basename(f))
        print '/img/post/' + os.path.basename(f) + ' -> /img/post/' + os.path.basename(f)

    # Upload all the HTML files
    for f in glob.glob(web_root + '/*.html'):
        sftp.put(f, remotepath + '/' + os.path.basename(f))
        print '/' + os.path.basename(f) + ' -> /' + os.path.basename(f)

    # Upload the RSS feed XML
    sftp.put(web_root + '/rss.xml', remotepath + '/rss.xml')

    print 'Publish complete'