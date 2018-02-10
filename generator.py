"""Generate HTML from Markdown source files."""

import codecs
import datetime
import glob
import os
import re
import markdown

def parse_iso8601_date(date_string):
    """Parse an ISO 8601 date string and return a date object."""
    return datetime.datetime.strptime(date_string, "%Y-%m-%d %H:%M")

def is_static_page(page_metadata):
    """Returns true if the page is a static page (not a post)."""
    return page_metadata["page_type"] == "static"

def get_header_string(header_regex, content):
    """Get the value retrieved from the specified header."""
    match = header_regex.search(content)
    return match.group(2).strip() if match else None

def get_header_date(header_regex, content):
    """Get the value retrieved from the specified header."""
    header_string = get_header_string(header_regex, content)
    return parse_iso8601_date(header_string) if header_string else datetime.datetime.now()

def strip_post_metadata(header_regex, content):
    """Strip the headers from the content of a post."""
    no_metadata = header_regex["title"].sub("", content)
    no_metadata = header_regex["published"].sub("", no_metadata)
    no_metadata = header_regex["updated"].sub("", no_metadata)
    no_metadata = header_regex["abstract"].sub("", no_metadata)
    no_metadata = header_regex["page_type"].sub("", no_metadata)
    no_metadata = header_regex["thumbnail"].sub("", no_metadata)
    no_metadata = header_regex["template"].sub("", no_metadata)
    no_metadata = header_regex["document_class"].sub("", no_metadata)
    return no_metadata

def parse_page_data(header_regex, content, source_filename, output_filename, cdn_url):
    """Parse headers (date and title) from page file content."""
    abstract_text = get_header_string(header_regex["abstract"], content)

    more_link = " <a class=\"more-link\" href=\"{0}\">&rarr;</a>".format(output_filename)

    content_no_metadata = strip_post_metadata(header_regex, content)
    body = re.sub(r"(\$\{cdn2\})", cdn_url, content_no_metadata)

    return {
        "title": get_header_string(header_regex["title"], content),
        "published": get_header_date(header_regex["published"], content),
        "updated": get_header_date(header_regex["updated"], content),
        "body": markdown.markdown(body, extensions=["extra", "codehilite"]),
        "abstract": markdown.markdown(abstract_text + more_link) if abstract_text else None,
        "abstract_plain": abstract_text if abstract_text else None,
        "abstract_nolink": markdown.markdown(abstract_text) if abstract_text else None,
        "page_type": get_header_string(header_regex["page_type"], content),
        "thumbnail": get_header_string(header_regex["thumbnail"], content),
        "template": get_header_string(header_regex["template"], content),
        "source_filename": source_filename,
        "output_filename": output_filename,
        "document_class": get_header_string(header_regex["document_class"], content)
    }

def get_page_metadata(source_file_spec, header_regex, cdn_url):
    """Build a sorted list of page metadata."""
    page_metadata = []

    for path in glob.glob(source_file_spec):
        # Get the filename portion of the path
        source_filename = os.path.split(path)[1]
        # Replace the .md extension with .html to get the output filename
        output_filename = re.sub(r"(?si)^(.*\.)(md)$", r"\1html", source_filename)
        markdown_source = codecs.open(path, "r", "utf-8")
        post_data = parse_page_data(
            header_regex,
            markdown_source.read(),
            source_filename,
            output_filename,
            cdn_url
        )
        page_metadata.append(post_data)

    # Sort the file list by updated date descending
    return sorted(page_metadata, key=lambda k: k["updated"], reverse=True)

def get_most_recent(count, page_data_list):
    """Get the most recent N items from a list of page metadata."""
    most_recent = []
    for page_metadata in page_data_list:
        if len(most_recent) < count and not is_static_page(page_metadata):
            most_recent.append(page_metadata)
    return most_recent

def render_page(templates, template_data, data):
    """Render a page template using the specified data."""
    config = template_data["config"]
    meta_title = "{} - {}".format(data["title"], config["site_name"])
    template_name = data["template"] if data["template"] is not None else "post"
    return templates[template_name].render(
        published=data["published"],
        updated=data["updated"],
        title=data["title"],
        permalink=data["output_filename"],
        body=data["body"],
        post_list=template_data["post_list"],
        meta_title=meta_title,
        og_title=data["title"],
        og_abstract=data["abstract_plain"],
        og_image=data["thumbnail"] if data["thumbnail"] is not None else "site.png",
        og_url=data["output_filename"] if data["output_filename"] != "index.html" else "",
        site_url=config["site_url"],
        rss_description=config["rss_description"],
        comments=None if is_static_page(data) else 1,
        cdn1=config["cdn1"],
        cdn2=config["cdn2"],
        analytics_id=config["analytics_id"],
        disqus_id=config["disqus_id"],
        document_class=data["document_class"] if data["document_class"] is not None else "html-base"
    )
