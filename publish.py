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

def get_config():
    """Load configuration from files."""
    parser = configparser.RawConfigParser()
    parser.read('config.cfg')
    parser.read('versions.cfg')

    return {
        'hostname': parser.get('Site', 'hostname'),
        'cdn1': parser.get('Site', 'cdn1'),
        'cdn2': parser.get('Site', 'cdn2'),
        'analytics_id': parser.get('Site', 'analytics_id'),
        'disqus_id': parser.get('Site', 'disqus_id'),
        'asset_version': parser.get('Versions', 'asset_version'),
        'output_folder': 'public'
    }

def load_templates():
    """Load Jinja templates"""
    environment = Environment(loader=FileSystemLoader('templates/'))

    return {
        'index': environment.get_template('index.html'),
        'post': environment.get_template('post.html')
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

def get_post_data(header_regex, content, source_filename, output_filename, cdn_url):
    """Parse headers (date and title) from post file content."""
    abstract_text = get_header_string(header_regex['abstract'], content)

    more_link = ' <a class="more-link" href="' + output_filename + '">&rarr;</a>'

    content_no_metadata = strip_post_metadata(header_regex, content)
    body = re.sub(r"(\$\{cdn2\})", cdn_url, content_no_metadata)

    return {
        'title': get_header_string(header_regex['title'], content),
        'published': get_header_date(header_regex['published'], content),
        'updated': get_header_date(header_regex['updated'], content),
        'body': markdown.markdown(body, extensions=['extra', 'codehilite']),
        'abstract': markdown.markdown(abstract_text + more_link) if abstract_text else None,
        'abstract_plain': abstract_text if abstract_text else None,
        'abstract_nolink': markdown.markdown(abstract_text) if abstract_text else None,
        'pagetype': get_header_string(header_regex['pagetype'], content),
        'thumbnail': get_header_string(header_regex['thumbnail'], content),
        'markdown_file': source_filename,
        'html_file': output_filename
    }

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


HEADER_REGEX_FLAGS = re.IGNORECASE | re.MULTILINE

HEADER_REGEX = {
    'title': re.compile("(^Title: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    'published': re.compile("(^Published: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    'updated': re.compile("(^Updated: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    'abstract': re.compile("(^Abstract: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    'pagetype': re.compile("(^PageType: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    'thumbnail': re.compile("(^Thumbnail: (.*)[\r\n]+)", HEADER_REGEX_FLAGS)
}

CONFIG = get_config()

TEMPLATES = load_templates()

delete_files(CONFIG["output_folder"] + '/css/*.css')
delete_files(CONFIG["output_folder"] + '/js/*.js')
delete_files(CONFIG["output_folder"] + '/*.html')

# Concatenate minified CSS and JS files
concatenate_files('css/*.min.css', CONFIG["output_folder"] + '/css/all.min.v' + CONFIG["asset_version"] + '.css')
concatenate_files('js/*.min.js', CONFIG["output_folder"] + '/js/all.min.v' + CONFIG["asset_version"] + '.js')

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
    post_data = get_post_data(HEADER_REGEX, md.read(), markdown_file, html_file, CONFIG["cdn2"])
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

    post_content = TEMPLATES["post"].render(
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
        asset_version=CONFIG["asset_version"],
        cdn1=CONFIG["cdn1"],
        cdn2=CONFIG["cdn2"],
        analytics_id=CONFIG["analytics_id"],
        disqus_id=CONFIG["disqus_id"]
    )

    write_file_utf8(CONFIG["output_folder"] + '/' + inputfile['html_file'], post_content)

BASE_TITLE = 'Mark Ashley Bell, Freelance Web Designer/Developer'

# Create the index page, passing in the joined HTML for the homepage posts
OUTPUT = TEMPLATES["index"].render(
    posts=HOMEPAGE_POSTS,
    nav_items=NAV_ITEMS,
    meta_title=BASE_TITLE + ' - C# ASP.NET, jQuery, JavaScript and Python web development',
    og_title=BASE_TITLE,
    og_abstract='C# ASP.NET, jQuery, JavaScript and Python web development',
    og_image='site.png',
    og_url='',
    asset_version=CONFIG["asset_version"],
    cdn1=CONFIG["cdn1"],
    cdn2=CONFIG["cdn2"],
    analytics_id=CONFIG["analytics_id"],
    disqus_id=CONFIG["disqus_id"]
)

write_file_utf8(CONFIG["output_folder"] + '/index.html', OUTPUT)

# Generate the RSS feed XML
RSS_XML = create_rss_xml(
    title="Mark Ashley Bell",
    link="https://" + CONFIG["hostname"],
    description="The latest articles from " + CONFIG["hostname"],
    last_build_date=datetime.datetime.now(),
    rss_url="https://" + CONFIG["hostname"] + "/rss.xml",
    items=RSS_POSTS
)

write_file_utf8(CONFIG["output_folder"] + '/rss.xml', RSS_XML)

print('File generation complete')
