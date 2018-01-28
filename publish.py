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
    """Load Jinja templates."""
    environment = Environment(loader=FileSystemLoader('templates/'))

    return {
        'index': environment.get_template('index.html'),
        'post': environment.get_template('post.html')
    }

def parse_iso8601_date(date_string):
    """Parse an ISO 8601 date string and return a date object."""
    return datetime.datetime.strptime(date_string, '%Y-%m-%d %H:%M')

def is_static_page(page_metadata):
    """Returns true if the page is a static page (not a post)."""
    return page_metadata['pagetype'] == 'static'

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

def parse_page_data(header_regex, content, source_filename, output_filename, cdn_url):
    """Parse headers (date and title) from page file content."""
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
        'source_filename': source_filename,
        'output_filename': output_filename
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

def get_page_metadata(source_file_spec, header_regex):
    """Build a sorted list of page metadata."""
    page_metadata = []

    for path in glob.glob(source_file_spec):
        # Get the filename portion of the path
        source_filename = os.path.split(path)[1]
        # Replace the .md extension with .html to get the output filename
        output_filename = re.sub(r"(?si)^(.*\.)(md)$", r"\1html", source_filename)
        markdown_source = codecs.open(path, 'r', 'utf-8')
        post_data = parse_page_data(
            header_regex,
            markdown_source.read(),
            source_filename,
            output_filename,
            CONFIG["cdn2"]
        )
        page_metadata.append(post_data)

    # Sort the file list by updated date descending
    return sorted(page_metadata, key=lambda k: k['updated'], reverse=True)

def get_most_recent(count, page_data_list):
    """Get the most recent N items from a list of page metadata."""
    most_recent = []
    for page_metadata in page_data_list:
        if len(most_recent) < count and not is_static_page(page_metadata):
            most_recent.append(page_metadata)
    return most_recent

def render_page(template, data, nav_items, config, base_title):
    """Render a page template using the specified data."""
    return template.render(
        published=data['published'],
        updated=data['updated'],
        title=data['title'],
        permalink=data['output_filename'],
        body=data['body'],
        nav_items=nav_items,
        meta_title=data['title'] + base_title,
        og_title=data['title'],
        og_abstract=data['abstract_plain'],
        og_image=data['thumbnail'] if data['thumbnail'] is not None else 'site.png',
        og_url=data['output_filename'],
        comments=None if is_static_page(data) else 1,
        asset_version=config["asset_version"],
        cdn1=config["cdn1"],
        cdn2=config["cdn2"],
        analytics_id=config["analytics_id"],
        disqus_id=config["disqus_id"]
    )

def render_index(template, data, nav_items, config, base_title):
    """Render an index template using the specified data."""
    return template.render(
        posts=data,
        nav_items=nav_items,
        meta_title=base_title + ' - C# ASP.NET, jQuery, JavaScript and Python web development',
        og_title=base_title,
        og_abstract='C# ASP.NET, jQuery, JavaScript and Python web development',
        og_image='site.png',
        og_url='',
        asset_version=config["asset_version"],
        cdn1=config["cdn1"],
        cdn2=config["cdn2"],
        analytics_id=config["analytics_id"],
        disqus_id=config["disqus_id"]
    )

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

concatenate_files('css/*.min.css', CONFIG["output_folder"] + '/css/all.min.v' + CONFIG["asset_version"] + '.css')
concatenate_files('js/*.min.js', CONFIG["output_folder"] + '/js/all.min.v' + CONFIG["asset_version"] + '.js')

PAGE_DATA_LIST = get_page_metadata('posts/*.md', HEADER_REGEX)

# Build a list of nav links to individual posts (which are already
# ordered by created date desc, so we're adding in the correct order)
NAV_ITEMS = [p for p in PAGE_DATA_LIST if not is_static_page(p)]

for page_data in PAGE_DATA_LIST:
    base_page_title = ' - Mark Ashley Bell'
    page_content = render_page(TEMPLATES["post"], page_data, NAV_ITEMS, CONFIG, base_page_title)
    write_file_utf8(CONFIG["output_folder"] + '/' + page_data['output_filename'], page_content)

BASE_TITLE = 'Mark Ashley Bell, Freelance Web Designer/Developer'

INDEX = render_index(TEMPLATES["index"], get_most_recent(5, PAGE_DATA_LIST), NAV_ITEMS, CONFIG, BASE_TITLE)

write_file_utf8(CONFIG["output_folder"] + '/index.html', INDEX)

RSS_XML = create_rss_xml(
    title="Mark Ashley Bell",
    link="https://" + CONFIG["hostname"],
    description="The latest articles from " + CONFIG["hostname"],
    last_build_date=datetime.datetime.now(),
    rss_url="https://" + CONFIG["hostname"] + "/rss.xml",
    items=get_most_recent(10, PAGE_DATA_LIST)
)

write_file_utf8(CONFIG["output_folder"] + '/rss.xml', RSS_XML)

print('File generation complete')
