"""Generate HTML in /public from Markdown source files in /posts."""

import datetime
import re
from generator import get_page_metadata, is_static_page, render_index, render_page, get_most_recent
from ioutils import get_config, load_templates, delete_files, concatenate_files, write_file_utf8
from rss import create_rss_xml

HEADER_REGEX_FLAGS = re.IGNORECASE | re.MULTILINE

HEADER_REGEX = {
    "title": re.compile("(^Title: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    "published": re.compile("(^Published: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    "updated": re.compile("(^Updated: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    "abstract": re.compile("(^Abstract: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    "pagetype": re.compile("(^PageType: (.*)[\r\n]+)", HEADER_REGEX_FLAGS),
    "thumbnail": re.compile("(^Thumbnail: (.*)[\r\n]+)", HEADER_REGEX_FLAGS)
}

CONFIG = get_config()

TEMPLATES = load_templates()

delete_files(CONFIG["output_folder"] + "/css/*.css")
delete_files(CONFIG["output_folder"] + "/js/*.js")
delete_files(CONFIG["output_folder"] + "/*.html")

concatenate_files("css/*.min.css", CONFIG["output_folder"] + "/css/all.min.v" + CONFIG["asset_version"] + ".css")
concatenate_files("js/*.min.js", CONFIG["output_folder"] + "/js/all.min.v" + CONFIG["asset_version"] + ".js")

PAGE_DATA_LIST = get_page_metadata("posts/*.md", HEADER_REGEX, CONFIG["cdn2"])

# Build a list of nav links to individual posts (which are already
# ordered by created date desc, so we"re adding in the correct order)
NAV_ITEMS = [p for p in PAGE_DATA_LIST if not is_static_page(p)]

PAGE_TEMPLATE_DATA = {
    "template": TEMPLATES["post"],
    "config": CONFIG,
    "nav_items": NAV_ITEMS,
    "base_title": " - Mark Ashley Bell"
}

for page_data in PAGE_DATA_LIST:
    page_content = render_page(PAGE_TEMPLATE_DATA, page_data)
    write_file_utf8(CONFIG["output_folder"] + "/" + page_data["output_filename"], page_content)

INDEX_TITLE = "Mark Ashley Bell, Freelance Web Designer/Developer"
INDEX_ABSTRACT = "C# ASP.NET, jQuery, JavaScript and Python web development"

INDEX_TEMPLATE_DATA = {
    "template": TEMPLATES["index"],
    "config": CONFIG,
    "nav_items": NAV_ITEMS,
    "title": INDEX_TITLE,
    "meta_title": "{} - {}".format(INDEX_TITLE, INDEX_ABSTRACT),
    "abstract": INDEX_ABSTRACT
}

INDEX = render_index(INDEX_TEMPLATE_DATA, get_most_recent(5, PAGE_DATA_LIST))

write_file_utf8(CONFIG["output_folder"] + "/index.html", INDEX)

RSS_TEMPLATE_DATA = {
    "title": "Mark Ashley Bell",
    "link": "https://" + CONFIG["hostname"],
    "description": "The latest articles from " + CONFIG["hostname"],
    "last_build_date": datetime.datetime.now(),
    "rss_url": "https://" + CONFIG["hostname"] + "/rss.xml",
}

RSS_XML = create_rss_xml(RSS_TEMPLATE_DATA, get_most_recent(10, PAGE_DATA_LIST))

write_file_utf8(CONFIG["output_folder"] + "/rss.xml", RSS_XML)

print("File generation complete")
