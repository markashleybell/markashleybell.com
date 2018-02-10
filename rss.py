"""RSS utility functions."""

from datetime import datetime
from xml.dom.minidom import Document
import markdown

def unix_timestamp(date):
    """Returns total number of seconds since epoch."""
    epoch = datetime.utcfromtimestamp(0)
    delta = date - epoch
    return int(delta.total_seconds())

def rfc822_date(date):
    """Format a date according to RFC 822"""
    days = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"]
    months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]
    date_format = "{:s}, {:02d} {:s} {:04d} {:02d}:{:02d}:{:02d} GMT"
    return date_format.format(
        days[date.weekday()],
        date.day, months[date.month-1],
        date.year,
        date.hour,
        date.minute,
        date.second
    )

def create_xml_element(xml, element_name, text=None):
    """Create an XML element containing the specified text."""
    element = xml.createElement(element_name)
    if text:
        element.appendChild(xml.createTextNode(text))
    return element

def create_rss_item_xml(xml, base_url, item_data):
    """Generate RSS feed item XML from metadata."""
    item_element = create_xml_element(xml, "item")

    title_element = create_xml_element(xml, "title", item_data["title"])
    link_element = create_xml_element(xml, "link", base_url + "/" + item_data["output_filename"])

    description_html = markdown.markdown(item_data["abstract"], extensions=["extra"])
    description_element = create_xml_element(xml, "description", description_html)

    # Manually updating the "updated" header value for a post
    # will cause readers to re-download the item
    time_stamp = "{0:d}".format(unix_timestamp(item_data["updated"]))
    guid = base_url + "/" + item_data["output_filename"] + "?d=" + time_stamp
    guid_element = create_xml_element(xml, "guid", guid)
    guid_element.setAttribute("isPermaLink", "false")

    pub_date_element = create_xml_element(xml, "pubDate", rfc822_date(item_data["updated"]))

    item_element.appendChild(title_element)
    item_element.appendChild(link_element)
    item_element.appendChild(description_element)
    item_element.appendChild(guid_element)
    item_element.appendChild(pub_date_element)

    return item_element

def create_rss_xml(template_data, items):
    """Generate RSS feed XML from metadata."""
    last_build_date = rfc822_date(template_data["last_build_date"])

    xml = Document()

    rss_element = create_xml_element(xml, "rss")
    rss_element.setAttribute("version", "2.0")
    rss_element.setAttribute("xmlns:atom", "http://www.w3.org/2005/Atom")

    channel_element = create_xml_element(xml, "channel")
    title_element = create_xml_element(xml, "title", template_data["title"])
    link_element = create_xml_element(xml, "link", template_data["link"])
    description_element = create_xml_element(xml, "description", template_data["description"])
    last_build_date_element = create_xml_element(xml, "lastBuildDate", last_build_date)

    atom_link_element = create_xml_element(xml, "atom:link")
    atom_link_element.setAttribute("href", template_data["rss_url"])
    atom_link_element.setAttribute("rel", "self")
    atom_link_element.setAttribute("type", "application/rss+xml")

    channel_element.appendChild(title_element)
    channel_element.appendChild(link_element)
    channel_element.appendChild(description_element)
    channel_element.appendChild(last_build_date_element)
    channel_element.appendChild(atom_link_element)

    for i in items:
        channel_element.appendChild(create_rss_item_xml(xml, template_data["link"], i))

    rss_element.appendChild(channel_element)

    xml.appendChild(rss_element)

    return xml.toxml()
