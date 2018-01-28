"""Generate HTML in /public from Markdown source files in /posts."""

import codecs
import configparser
import datetime
import fileinput
import glob
import os
import re
import markdown
from jinja2 import Environment, FileSystemLoader
from rss import create_rss_xml


def get_post_data(content, source_filename, output_filename):
    """Parse headers (date and title) from post file content."""
    metadata = {
        'title': None,
        'published': None,
        'updated': None,
        'body': None,
        'abstract': None,
        'abstract_plain': None,
        'abstract_nolink': None,
        'pagetype': None,
        'thumbnail': None,
        'markdown_file': source_filename,
        'html_file': output_filename
    }
    cdn2_regex = r"(\$\{cdn2\})"
    regex = {
        'title': re.compile("(^Title: (.*)[\r\n]+)", re.IGNORECASE | re.MULTILINE),
        'published': re.compile("(^Published: (.*)[\r\n]+)", re.IGNORECASE | re.MULTILINE),
        'updated': re.compile("(^Updated: (.*)[\r\n]+)", re.IGNORECASE | re.MULTILINE),
        'abstract': re.compile("(^Abstract: (.*)[\r\n]+)", re.IGNORECASE | re.MULTILINE),
        'pagetype': re.compile("(^PageType: (.*)[\r\n]+)", re.IGNORECASE | re.MULTILINE),
        'thumbnail': re.compile("(^Thumbnail: (.*)[\r\n]+)", re.IGNORECASE | re.MULTILINE)
    }
    match = regex['title'].search(content)
    if match:
        metadata['title'] = match.group(2).strip()
    match = regex['published'].search(content)
    if match:
        metadata['published'] = datetime.datetime.strptime(match.group(2).strip(), '%Y-%m-%d %H:%M')
    match = regex['updated'].search(content)
    if match:
        metadata['updated'] = datetime.datetime.strptime(match.group(2).strip(), '%Y-%m-%d %H:%M')
    match = regex['abstract'].search(content)
    if match:
        abstract_text = match.group(2).strip()
        more_link = ' <a class="more-link" href="' + output_filename + '">&rarr;</a>'
        abstract_text_with_link = abstract_text + more_link
        metadata['abstract_plain'] = re.sub(cdn2_regex, CDN2, abstract_text)
        metadata['abstract_nolink'] = markdown.markdown(re.sub(cdn2_regex, CDN2, abstract_text))
        metadata['abstract'] = markdown.markdown(re.sub(cdn2_regex, CDN2, abstract_text_with_link))
    match = regex['pagetype'].search(content)
    if match:
        metadata['pagetype'] = match.group(2).strip()
    match = regex['thumbnail'].search(content)
    if match:
        metadata['thumbnail'] = match.group(2).strip()
    # Remove the header lines if they were present
    content_no_metadata = regex['title'].sub('', content)
    content_no_metadata = regex['published'].sub('', content_no_metadata)
    content_no_metadata = regex['updated'].sub('', content_no_metadata)
    content_no_metadata = regex['abstract'].sub('', content_no_metadata)
    content_no_metadata = regex['pagetype'].sub('', content_no_metadata)
    content_no_metadata = regex['thumbnail'].sub('', content_no_metadata)
    # Populate the body field
    body = re.sub(cdn2_regex, CDN2, content_no_metadata)
    metadata['body'] = markdown.markdown(body, extensions=['extra', 'codehilite'])
    return metadata


# Load config
CONFIG = configparser.RawConfigParser()
CONFIG.read('config.cfg')
CONFIG.read('versions.cfg')

HOSTNAME = CONFIG.get('Site', 'hostname')
CDN1 = CONFIG.get('Site', 'cdn1')
CDN2 = CONFIG.get('Site', 'cdn2')
ANALYTICS_ID = CONFIG.get('Site', 'analytics_id')
DISQUS_ID = CONFIG.get('Site', 'disqus_id')
# Asset file version (for breaking cache)
ASSET_VERSION = CONFIG.get('Versions', 'asset_version')
# Define web root (html output) folder
WEB_ROOT = 'public'

# Load the templates
ENV = Environment(loader=FileSystemLoader('templates/'))
INDEX_TEMPLATE = ENV.get_template('index.html')
POST_TEMPLATE = ENV.get_template('post.html')

# Remove old files
for f in glob.glob(WEB_ROOT + '/css/*.css'):
    os.remove(f)
for f in glob.glob(WEB_ROOT + '/js/*.js'):
    os.remove(f)
for f in glob.glob(WEB_ROOT + '/*.html'):
    os.remove(f)

# Concatenate all minified CSS files
CSS_FILES = [f for f in glob.glob('css/*.min.css')]
with open(WEB_ROOT + '/css/all.min.v' + ASSET_VERSION + '.css', 'w') as fout:
    for line in fileinput.input(CSS_FILES):
        fout.write(line)

# Concatenate all minified JS files
JS_FILES = [f for f in glob.glob('js/*.min.js')]
with open(WEB_ROOT + '/js/all.min.v' + ASSET_VERSION + '.js', 'w') as fout:
    for line in fileinput.input(JS_FILES):
        fout.write(line)

# Build a sortable list of file information
FILE_LIST = []

# Loop through all Markdown files in the posts folder
for f in glob.glob('posts/*.md'):
    # Get the filename portion of the path
    markdown_file = os.path.split(f)[1]
    # Replace the .md extension with .html to get the output filename
    html_file = re.sub(r"(?si)^(.*\.)(md)$", r"\1html", markdown_file)
    # Open the Markdown file and get the first line (heading)
    md = codecs.open(f, 'r', 'utf-8')
    post_data = get_post_data(md.read(), markdown_file, html_file)
    FILE_LIST.append(post_data)

# Sort the file list by post date descending
FILE_LIST = sorted(FILE_LIST, key=lambda k: k['updated'], reverse=True)

# Just delete all existing HTML files to avoid orphans
for f in glob.glob(WEB_ROOT + '/*.html'):
    os.unlink(f)

# Build a list of nav links to individual posts
# (we are ordered by created date descending, so we're adding in the correct order)
NAV_ITEMS = [f for f in FILE_LIST if f['pagetype'] != 'static']

# Collect the first n posts to display on the home page
HOMEPAGE_POSTS = []
HOMEPAGE_POST_COUNT = 5
RSS_POSTS = []
RSS_POST_COUNT = 10

for inputfile in FILE_LIST:
    # If there are less than 5 posts in the homepage list, add this one
    if len(HOMEPAGE_POSTS) < HOMEPAGE_POST_COUNT and inputfile['pagetype'] != 'static':
        HOMEPAGE_POSTS.append(inputfile)
    # Add the raw post details to the RSS feed
    if len(RSS_POSTS) < RSS_POST_COUNT and inputfile['pagetype'] != 'static':
        RSS_POSTS.append(inputfile)
    # Populate the post template with the post data
    comments = None if inputfile['pagetype'] == 'static' else 1
    output = POST_TEMPLATE.render(
        published=inputfile['published'],
        updated=inputfile['updated'],
        title=inputfile['title'],
        permalink=inputfile['html_file'],
        body=inputfile['body'],
        nav_items=NAV_ITEMS,
        meta_title=inputfile['title'] + ' - Mark Ashley Bell',
        og_title=inputfile['title'],
        og_abstract=inputfile['abstract_plain'],
        og_image=inputfile['thumbnail'] if inputfile['thumbnail'] is not None else 'site.png',
        og_url=inputfile['html_file'],
        comments=comments,
        asset_version=ASSET_VERSION,
        cdn1=CDN1,
        cdn2=CDN2,
        analytics_id=ANALYTICS_ID,
        disqus_id=DISQUS_ID
    )
    # Write out the processed HTML file for this post
    o = codecs.open(WEB_ROOT + '/' + inputfile['html_file'], 'w', 'utf-8')
    o.write(output)
    o.close()

BASE_TITLE = 'Mark Ashley Bell, Freelance Web Designer/Developer'

# Create the index page, passing in the joined HTML for the homepage posts
OUTPUT = INDEX_TEMPLATE.render(
    posts=HOMEPAGE_POSTS,
    nav_items=NAV_ITEMS,
    meta_title=BASE_TITLE + ' - C# ASP.NET, jQuery, JavaScript and Python web development',
    og_title=BASE_TITLE,
    og_abstract='C# ASP.NET, jQuery, JavaScript and Python web development',
    og_image='site.png',
    og_url='',
    asset_version=ASSET_VERSION,
    cdn1=CDN1,
    cdn2=CDN2,
    analytics_id=ANALYTICS_ID,
    disqus_id=DISQUS_ID
)
# Write out the processed HTML file for the index page
O = codecs.open(WEB_ROOT + '/index.html', 'w', 'utf-8')
O.write(OUTPUT)
O.close()

# Generate the RSS feed XML
RSS_XML = create_rss_xml(
    title="Mark Ashley Bell",
    link="https://" + HOSTNAME,
    description="The latest articles from " + HOSTNAME,
    last_build_date=datetime.datetime.now(),
    rss_url="https://" + HOSTNAME + "/rss.xml",
    items=RSS_POSTS
)

# Write out the RSS XML to a file
F = codecs.open(WEB_ROOT + '/rss.xml', 'w', 'utf-8')
RSS_XML.writexml(F)
F.close()

print('File generation complete')
