"""Generate HTML in /public from Markdown source files in /posts."""

import datetime
import re
from generator import get_page_metadata, is_static_page, render_index, render_page, get_most_recent
from ioutils import get_config, load_templates, delete_files, write_file_utf8
from rss import create_rss_xml

HEADER_REGEX_FLAGS = re.IGNORECASE | re.MULTILINE

HEADER_REGEX = {
    "title": re.compile("(^Title: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    "published": re.compile("(^Published: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    "updated": re.compile("(^Updated: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    "abstract": re.compile("(^Abstract: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    "pagetype": re.compile("(^PageType: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    "thumbnail": re.compile("(^Thumbnail: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    "template": re.compile("(^Template: (.*)[\r\n]+)", HEADER_REGEX_FLAGS)
}

CONFIG = get_config()

TEMPLATES = load_templates()

delete_files(CONFIG["output_folder"] + "/*.html")

PAGE_DATA_LIST = get_page_metadata("posts/*.md", HEADER_REGEX, CONFIG["cdn2"])

# Build a list of nav links to individual posts (which are already
# ordered by created date desc, so we"re adding in the correct order)
NAV_ITEMS = [p for p in PAGE_DATA_LIST if not is_static_page(p)]

PAGE_TEMPLATE_DATA = {
    "config": CONFIG,
    "nav_items": NAV_ITEMS
}

for page_data in PAGE_DATA_LIST:
    page_content = render_page(TEMPLATES, PAGE_TEMPLATE_DATA, page_data)
    write_file_utf8(CONFIG["output_folder"] + "/" + page_data["output_filename"], page_content)

INDEX_TEMPLATE_DATA = {
    "template": TEMPLATES["index"],
    "config": CONFIG,
    "nav_items": NAV_ITEMS
}

INDEX = render_index(INDEX_TEMPLATE_DATA, get_most_recent(5, PAGE_DATA_LIST))

write_file_utf8(CONFIG["output_folder"] + "/index.html", INDEX)

RSS_TEMPLATE_DATA = {
    "title":  CONFIG["site_name"],
    "link": CONFIG["site_url"],
    "description": CONFIG["rss_description"],
    "last_build_date": datetime.datetime.now(),
    "rss_url": CONFIG["site_url"] + "/rss.xml"
}

RSS_XML = create_rss_xml(RSS_TEMPLATE_DATA, get_most_recent(10, PAGE_DATA_LIST))

write_file_utf8(CONFIG["output_folder"] + "/rss.xml", RSS_XML)

print("File generation complete")
