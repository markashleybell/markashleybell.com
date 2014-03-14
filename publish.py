# This Python file uses the following encoding: utf-8
import argparse, markdown, datetime, codecs, re, os, fileinput, glob, time, re, ConfigParser, rss
import paramiko, base64, getpass, socket, sys, traceback # Only needed for uploads
# from string import Template
from jinja2 import Template, Environment, FileSystemLoader
from xml.dom.minidom import Document

# Parse headers (date and title) from post file content
def get_post_data(content, markdown_file, html_file):
    # Fields are empty by default
    metadata = { 'title': None, 'published': None, 'updated': None, 'body': None, 'abstract': None, 'abstract_nolink': None, 'pagetype': None, 'markdown_file': markdown_file, 'html_file': html_file }
    # Try and get the title
    titlere = re.compile("(^Title: (.*)[\r\n]+)", re.IGNORECASE | re.MULTILINE)
    match = titlere.search(content)
    if match:
        metadata['title'] = match.group(2).strip()
    # Try and get the published date
    publishedre = re.compile("(^Published: (.*)[\r\n]+)", re.IGNORECASE | re.MULTILINE)
    match = publishedre.search(content)
    if match:
        metadata['published'] = datetime.datetime.strptime(match.group(2).strip(), '%Y-%m-%d %H:%M')
    # Try and get the updated date
    updatedre = re.compile("(^Updated: (.*)[\r\n]+)", re.IGNORECASE | re.MULTILINE)
    match = updatedre.search(content)
    if match:
        metadata['updated'] = datetime.datetime.strptime(match.group(2).strip(), '%Y-%m-%d %H:%M')
    # Try and get the abstract
    abstractre = re.compile("(^Abstract: (.*)[\r\n]+)", re.IGNORECASE | re.MULTILINE)
    match = abstractre.search(content)
    if match:
        metadata['abstract_nolink'] = markdown.markdown(re.sub(r"(\$\{cdn2\})", cdn2, match.group(2).strip()))
        metadata['abstract'] = markdown.markdown(re.sub(r"(\$\{cdn2\})", cdn2, match.group(2).strip() + ' <a class="more-link" href="' + html_file + '">&rarr;</a>'))
    # Try and get the type
    pagetypere = re.compile("(^PageType: (.*)[\r\n]+)", re.IGNORECASE | re.MULTILINE)
    match = pagetypere.search(content)
    if match:
        metadata['pagetype'] = match.group(2).strip()
    # Remove the header lines if they were present
    content_no_metadata = titlere.sub('', content)
    content_no_metadata = publishedre.sub('', content_no_metadata)
    content_no_metadata = updatedre.sub('', content_no_metadata)
    content_no_metadata = abstractre.sub('', content_no_metadata)
    content_no_metadata = pagetypere.sub('', content_no_metadata)
    # Populate the body field
    metadata['body'] = markdown.markdown(re.sub(r"(\$\{cdn2\})", cdn2, content_no_metadata), extensions=['extra', 'codehilite'])
    return metadata

# Set up command-line arguments
parser = argparse.ArgumentParser(description='Generate a blog from Markdown files and HTML templates')
parser.add_argument('-p', '--publish', help='publish to remote server', action='store_true')
args = parser.parse_args()

# Load config
config = ConfigParser.RawConfigParser()
config.read('config.cfg')

hostname = config.get('Site', 'hostname')
cdn1 = config.get('Site', 'cdn1')
cdn2 = config.get('Site', 'cdn2')
analytics_id = config.get('Site', 'analytics_id')
disqus_id = config.get('Site', 'disqus_id')
# Asset file version (for breaking cache)
asset_version = config.get('Site', 'asset_version')
# Determine whether to reference a single versioned css file
use_concatenated = config.getboolean('Debug', 'use_concatenated')
# Determine whether to use minified versions of scripts and CSS
minify = '.min' if config.getboolean('Debug', 'minify') else ''
# Define web root (html output) folder
web_root = 'public'

# Load the templates
env = Environment(loader=FileSystemLoader('templates/'))
index_template = env.get_template('index.html')
post_template = env.get_template('post.html')

# Concatenate all minified CSS files
css_files = [f for f in glob.glob(web_root + '/css/*.min.css')]
with open(web_root + '/css/all.min.v' + asset_version + '.css', 'w') as fout:
    for line in fileinput.input(css_files):
        fout.write(line)

# Concatenate all minified JS files
js_files = [f for f in glob.glob(web_root + '/js/*.min.js')]
with open(web_root + '/js/all.min.v' + asset_version + '.js', 'w') as fout:
    for line in fileinput.input(js_files):
        fout.write(line)

# Build a sortable list of file information
file_list = []

# Loop through all Markdown files in the posts folder
for f in glob.glob('posts/*.md'):
    # Get the filename portion of the path
    markdown_file = os.path.split(f)[1]
    # Replace the .md extension with .html to get the output filename
    html_file = re.sub(r"(?si)^(.*\.)(md)$", r"\1html", markdown_file)
    # Open the Markdown file and get the first line (heading)
    md = codecs.open(f, 'r', 'utf-8')
    post_data = get_post_data(md.read(), markdown_file, html_file)
    file_list.append(post_data)
 
# Sort the file list by post date descending 
file_list = sorted(file_list, key=lambda k: k['updated'], reverse = True) 

# Just delete all existing HTML files to avoid orphans
for f in glob.glob(web_root + '/*.html'):
   os.unlink (f)

# Build a list of nav links to individual posts 
# (we are ordered by created date descending, so we're adding in the correct order)
nav_items = [f for f in file_list if f['pagetype'] != 'static']

# Collect the first n posts to display on the home page
homepage_posts = []
homepage_post_count = 5
rss_posts = []
rss_post_count = 10

for inputfile in file_list:
    # If there are less than 5 posts in the homepage list, add this one
    if len(homepage_posts) < homepage_post_count and inputfile['pagetype'] != 'static':
        homepage_posts.append(inputfile)
    # Add the raw post details to the RSS feed
    if len(rss_posts) < rss_post_count and inputfile['pagetype'] != 'static': 
        rss_posts.append(inputfile)
    # Populate the post template with the post data
    comments = None if inputfile['pagetype'] == 'static' else 1
    output = post_template.render(published = inputfile['published'], 
        updated = inputfile['updated'], 
        title = inputfile['title'], 
        permalink = inputfile['html_file'], 
        body = inputfile['body'], 
        nav_items = nav_items, 
        meta_title = inputfile['title'] + ' - Mark Ashley Bell', 
        comments = comments, 
        minify = minify, 
        use_concatenated = use_concatenated,
        asset_version = asset_version,
        cdn1 = cdn1, 
        cdn2 = cdn2,
        analytics_id = analytics_id,
        disqus_id = disqus_id)
    # Write out the processed HTML file for this post
    o = codecs.open(web_root + '/' + inputfile['html_file'], 'w', 'utf-8')
    o.write(output)
    o.close()

# Create the index page, passing in the joined HTML for the homepage posts
output = index_template.render(posts = homepage_posts,
        nav_items = nav_items, 
        meta_title = 'Mark Ashley Bell, Freelance Web Designer/Developer - C# ASP.NET, jQuery, JavaScript and Python web development', 
        minify = minify, 
        use_concatenated = use_concatenated,
        asset_version = asset_version,
        cdn1 = cdn1, 
        cdn2 = cdn2,
        analytics_id = analytics_id,
        disqus_id = disqus_id)
# Write out the processed HTML file for the index page
o = codecs.open(web_root + '/index.html', 'w', 'utf-8')
o.write(output)
o.close()

# Generate the RSS feed XML
rss_feed = rss.RSSFeed(
        title = "Mark Ashley Bell",
        link = "http://" + hostname,
        description = "The latest articles from " + hostname,
        lastBuildDate = datetime.datetime.now(),
        atomLink = "http://" + hostname + "/rss.xml",
        items = rss_posts
    )
# Write out the RSS XML to a file
f = codecs.open(web_root + '/rss.xml', 'w', 'utf-8')
rss_feed.get_xml().writexml(f)
f.close()

print 'File generation complete'

secure = config.getboolean('FTP', 'secure')

if args.publish and secure:
    # Load config for upload
    hostname = config.get('FTP', 'hostname')
    username = config.get('FTP', 'username')
    password = config.get('FTP', 'password')
    remotepath = config.get('FTP', 'remotepath')

    # Set up log
    paramiko.util.log_to_file('sftp.log')

    # Connect to the server
    ssh = paramiko.SSHClient()
    ssh.set_missing_host_key_policy(paramiko.AutoAddPolicy())
    ssh.connect(hostname, username=username, password=password)
    sftp = ssh.open_sftp()

    public_folder = 'public'
    excludes = ['.gitignore']
    remote_folders = []

    for root, dirs, files in os.walk(public_folder):
        files = [name for name in files if name not in excludes]
        for name in files:
            upload_folder = ''
            if root != public_folder:
                upload_folder = '/' + '/'.join(root.replace('\\', '/').split('/')[1:])
            # Create the remote folder if it doesn't exist
            if upload_folder not in remote_folders:
                try:
                    s = sftp.stat(remotepath + upload_folder)
                except IOError as e:
                    print 'Creating folder ' + upload_folder
                    sftp.mkdir(remotepath + upload_folder)
                    # Store the remote name so we don't keep checking
                    remote_folders.append(upload_folder)
            sftp.put(os.path.join(root, name), remotepath + upload_folder + '/' + name)
            print 'Upload ' + os.path.join(root, name) + ' to ' + upload_folder + '/' + name

    print 'Publish complete'