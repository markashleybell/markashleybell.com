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


HEADER_REGEX_FLAGS = re.IGNORECASE | re.MULTILINE

HEADER_REGEX = {
    'title': re.compile("(^Title: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    'published': re.compile("(^Published: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    'updated': re.compile("(^Updated: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    'abstract': re.compile("(^Abstract: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    'pagetype': re.compile("(^PageType: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    'thumbnail': re.compile("(^Thumbnail: (.*)[\r\n]+)", HEADER_REGEX_FLAGS)
}

def parse_iso8601_date(date_string):
    """Parse an ISO 8601 date string and return a date object."""
    return datetime.datetime.strptime(date_string, '%Y-%m-%d %H:%M')

def get_header_string(header_regex, content):
    """Get the value retrieved from the specified header."""
    match = header_regex.search(content)
    return match.group(2).strip() if match else None

def get_header_date(header_regex, content):
    """Get the value retrieved from the specified header."""
    header_string = get_header_string(header_regex, content)
    return parse_iso8601_date(header_string) if header_string else None

def strip_post_metadata(header_regex, content):
    """Strip the headers from the content of a post."""
    no_metadata = header_regex['title'].sub('', content)
    no_metadata = header_regex['published'].sub('', no_metadata)
    no_metadata = header_regex['updated'].sub('', no_metadata)
    no_metadata = header_regex['abstract'].sub('', no_metadata)
    no_metadata = header_regex['pagetype'].sub('', no_metadata)
    no_metadata = header_regex['thumbnail'].sub('', no_metadata)
    return no_metadata

def get_post_data(header_regex, content, source_filename, output_filename):
    """Parse headers (date and title) from post file content."""
    metadata = {
        'title': get_header_string(header_regex['title'], content),
        'published': get_header_date(header_regex['published'], content),
        'updated': get_header_date(header_regex['updated'], content),
        'body': None,
        'abstract': None,
        'abstract_plain': None,
        'abstract_nolink': None,
        'pagetype': get_header_string(header_regex['pagetype'], content),
        'thumbnail': get_header_string(header_regex['thumbnail'], content),
        'markdown_file': source_filename,
        'html_file': output_filename
    }
    cdn2_regex = r"(\$\{cdn2\})"

    abstract_text = get_header_string(header_regex['abstract'], content)

    if abstract_text:
        more_link = ' <a class="more-link" href="' + output_filename + '">&rarr;</a>'
        abstract_text_with_link = abstract_text + more_link
        metadata['abstract_plain'] = re.sub(cdn2_regex, CDN2, abstract_text)
        metadata['abstract_nolink'] = markdown.markdown(re.sub(cdn2_regex, CDN2, abstract_text))
        metadata['abstract'] = markdown.markdown(re.sub(cdn2_regex, CDN2, abstract_text_with_link))

    # Get a copy of the content with all headers removed
    content_no_metadata = strip_post_metadata(header_regex, content)
    body = re.sub(cdn2_regex, CDN2, content_no_metadata)

    metadata['body'] = markdown.markdown(body, extensions=['extra', 'codehilite'])

    return metadata


def write_file_utf8(output_filename, content):
    """Write content to the specified file with UTF-8 encoding."""
    codec = codecs.open(output_filename, 'w', 'utf-8')
    codec.write(content)
    codec.close()

def delete_files(file_spec):
    """Delete all files matching file_spec glob pattern."""
    for file_name in glob.glob(file_spec):
        os.remove(file_name)

def concatenate_files(file_spec, output_file_name):
    """Concatenate all files matching file_spec glob pattern."""
    files = [f for f in glob.glob(file_spec)]
    with open(output_file_name, 'w') as fout:
        for line in fileinput.input(files):
            fout.write(line)

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
delete_files(WEB_ROOT + '/css/*.css')
delete_files(WEB_ROOT + '/js/*.js')
delete_files(WEB_ROOT + '/*.html')

# Concatenate all minified CSS and JS files
concatenate_files('css/*.min.css', WEB_ROOT + '/css/all.min.v' + ASSET_VERSION + '.css')
concatenate_files('js/*.min.js', WEB_ROOT + '/js/all.min.v' + ASSET_VERSION + '.js')

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
    post_data = get_post_data(HEADER_REGEX, md.read(), markdown_file, html_file)
    FILE_LIST.append(post_data)

# Sort the file list by post date descending
FILE_LIST = sorted(FILE_LIST, key=lambda k: k['updated'], reverse=True)

# Build a list of nav links to individual posts (which are already
# ordered by created date desc, so we're adding in the correct order)
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

    post_content = POST_TEMPLATE.render(
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

    write_file_utf8(WEB_ROOT + '/' + inputfile['html_file'], post_content)

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

write_file_utf8(WEB_ROOT + '/index.html', OUTPUT)

# Generate the RSS feed XML
RSS_XML = create_rss_xml(
    title="Mark Ashley Bell",
    link="https://" + HOSTNAME,
    description="The latest articles from " + HOSTNAME,
    last_build_date=datetime.datetime.now(),
    rss_url="https://" + HOSTNAME + "/rss.xml",
    items=RSS_POSTS
)

write_file_utf8(WEB_ROOT + '/rss.xml', RSS_XML)

print('File generation complete')
