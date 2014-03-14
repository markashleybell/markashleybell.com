from xml.dom.minidom import Document
import datetime, time

class RSSChannel():
    def __init__(self,
                 title,
                 link,
                 description,
                 language = None,
                 copyright = None,
                 pubDate = None,  # a datetime, *in* *GMT*
                 lastBuildDate = None, # a datetime
                 docs = "http://blogs.law.harvard.edu/tech/rss",
                 items = None,     # list of RSSItems
                 ):
        self.title = title
        self.link = link
        self.description = description
        self.language = language
        self.copyright = copyright
        self.pubDate = pubDate
        self.lastBuildDate = lastBuildDate
        self.docs = docs
        self.items = items

    def _format_date(self, dt):
        return "%s, %02d %s %04d %02d:%02d:%02d GMT" % (
                ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"][dt.weekday()],
                dt.day,
                ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
                 "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"][dt.month-1],
                dt.year, dt.hour, dt.minute, dt.second)

    def get_xml(self):
        doc = Document()
        rss = doc.createElement('rss')
        rss.setAttribute('version', '2.0')
        rss.setAttribute('xmlns:atom', 'http://www.w3.org/2005/Atom')
        doc.appendChild(rss)
        channel = doc.createElement('channel')
        rss.appendChild(channel)
        title = doc.createElement('title')
        title.appendChild(doc.createTextNode(self.title));
        channel.appendChild(title)
        link = doc.createElement('link')
        link.appendChild(doc.createTextNode(self.link));
        channel.appendChild(link)
        description = doc.createElement('description')
        description.appendChild(doc.createTextNode(self.description));
        channel.appendChild(description)
        lastBuildDate = doc.createElement('lastBuildDate')
        lastBuildDate.appendChild(doc.createTextNode(self._format_date(self.lastBuildDate)));
        channel.appendChild(lastBuildDate)
        atomLink = doc.createElement('atom:link')
        atomLink.setAttribute('href', 'http://markashleybell.com/rss.xml')
        atomLink.setAttribute('rel', 'self')
        atomLink.setAttribute('type', 'application/rss+xml');
        channel.appendChild(atomLink)

        for i in self.items:
            item = doc.createElement('item')
            channel.appendChild(item)
            title = doc.createElement('title')
            title.appendChild(doc.createTextNode(i['title']))
            item.appendChild(title)

        return doc

    
        

# class RSSItem():
#     def __init__(self,
#                  title = None,  # string
#                  link = None,   # url as string
#                  description = None, # string
#                  guid = None,    # a unique string
#                  pubDate = None, # a datetime
#                  ):
#         self.title = title
#         self.link = link
#         self.description = description
#         self.guid = guid
#         self.pubDate = pubDate
