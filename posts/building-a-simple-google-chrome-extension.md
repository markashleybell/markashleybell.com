Title: Building a simple Google Chrome extension
Abstract: In this example, I walk through creating a simple extension for Google Chrome which grabs the title, url and any selected text from the current page.
Date: 2010-01-26 08:41

I have a web app running on my home server to keep track of my bookmarks—it's a little like [Delicious](http://delicious.com/), but simpler and with some personal customisations. Currently I save bookmarks to this app via a Javascript bookmarklet: clicking it gets the current page's title and url (and also any selected text, to use as a summary) and sends it to a popup form; submitting that form then saves the bookmark data to the server.

Although this system works well enough, it looks a bit untidy and takes up space in the bookmarks bar. With the advent of [Extensions for Chrome](http://www.theregister.co.uk/2010/01/25/google_chrome_4_stable/), I thought I'd have a go at writing an extension to nicely integrate my custom page bookmarking button into the Chrome browser.

![Screen Shot](${cdn2}/img/post/chrome-extension-screenshot.gif "Screen Shot")

It's clear from the start that Chrome's extension structure is a lot simpler than that of [Firefox extensions](http://kb.mozillazine.org/Getting_started_with_extension_development). Chrome extensions are just a collection of plain HTML and JavaScript files—no odd folder hierarchies or XUL to deal with here. Of course, there are several advantages to Mozilla's approach (ease of internationalisation, UI consistency), but I can't help feeling that building Chrome extensions will be much more accessible to amateur developers; I'm betting that this is exactly what Google was aiming for.

So let's get stuck in! First create a new folder for your extension code—it doesn't matter where for now. My basic Chrome extension consists of just a few files:

## manifest.json

This is the glue that holds our extension together. It contains the basic meta data about the extension (title, description etc), as well as acting as a pointer to the various files that contain the extension's user interface and JavaScript code. It also defines permissions that specify which browser components and external URLs the extension is allowed to access. The manifest for our extension looks like this:

    :::javascript
    {
        "name": "Bookmark",
        "description": "Adds the current page to my bookmarking system.",
        "version": "1.0",
        "background_page": "background.html",
        "permissions": [
            "tabs", 
            "http://*/*", 
            "https://*/*"
        ],
        "browser_action": {
            "default_title": "Bookmark This Page",
            "default_icon": "icon.png",
            "popup": "popup.html"
        }
    }

The `background_page` property points to an HTML page which contains the logic code for the extension. This HTML is never displayed, it just interacts with the browser and page via JavaScript. The `browser_action` section defines a button with an icon, which the user will click to open the bookmarking dialog, and the `popup` property which points to the HTML file containing the dialog form.

## popup.html

This file contains a basic HTML form with title, url, summary and tag fields (so that we can edit and tag our page bookmark before saving it), and some JavaScript code to do the population and saving of the fields. You can [download the complete source here](${cdn2}/files/mab_bookmark_extension.zip), but for now the important part is the script:

    :::javascript
    // This callback function is called when the content script has been 
    // injected and returned its results
    function onPageInfo(o) 
    { 
        document.getElementById("title").value = o.title; 
        document.getElementById("url").value = o.url; 
        document.getElementById("summary").innerText = o.summary; 
    } 

    // POST the data to the server using XMLHttpRequest
    function addBookmark(f)
    {
        var req = new XMLHttpRequest();
    	req.open("POST", "http://mywebappurl/do_add_bookmark/", true);
    	
    	var params = "title=" + document.getElementById("title").value + 
    				 "&url=" + document.getElementById("url").value + 
    				 "&summary=" + document.getElementById("summary").value +
    				 "&tags=" + document.getElementById("tags").value;
    	
    	req.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    	req.setRequestHeader("Content-length", params.length);
    	req.setRequestHeader("Connection", "close");
    	
    	req.send(params);
        
        req.onreadystatechange = function() 
        { 
            // If the request completed, close the extension popup
            if (req.readyState == 4)
                if (req.status == 200) window.close();
        };
        
        return false;
    }

    // Call the getPageInfo function in the background page, passing in 
    // our onPageInfo function as the callback
    window.onload = function() 
    { 
        var bg = chrome.extension.getBackgroundPage();
        bg.getPageInfo(onPageInfo);
    }

This may look a little confusing at first, but it will hopefully make more sense when you see the other code.

## background.html

Think of this file as the negotiator between the popup dialog and the content/DOM of the currently loaded web page. Though it's an HTML file, it only needs contain a single script tag (as shown below); it will never be displayed anywhere. `getPageInfo` is the function we called when our popup loaded, and its parameter is the callback function which sets the values of the form fields in `popup.html`.

    :::html
    <script>
        // Array to hold callback functions
        var callbacks = []; 
        
        // This function is called onload in the popup code
        function getPageInfo(callback) 
        { 
            // Add the callback to the queue
            callbacks.push(callback); 

            // Injects the content script into the current page 
            chrome.tabs.executeScript(null, { file: "content_script.js" }); 
        }; 

        // Perform the callback when a request is received from the content script
        chrome.extension.onRequest.addListener(function(request) 
        { 
            // Get the first callback in the callbacks array
            // and remove it from the array
            var callback = callbacks.shift();

            // Call the callback function
            callback(request); 
        }); 
    </script>


When `getPageInfo` is called, it pushes the callback function onto a queue and then injects the content script (below) into the code of the current web page.

## content_script.js

The content script itself is pretty simple: it just gets the title, url and any selected text from the current page and fires them back the the background page.

    :::javascript
    // Object to hold information about the current page
    var pageInfo = {
        "title": document.title,
        "url": window.location.href,
        "summary": window.getSelection().toString()
    };

    // Send the information back to the extension
    chrome.extension.sendRequest(pageInfo);

The background page listener then gets the callback function from the queue (which, if you remember, is the `onPageInfo` function from the popup page) and calls it, passing in the information about the page so that it can populate the form field values.

Testing and installing the extension is much easier than in Firefox, too. All you need to do is click the Chrome "spanner" icon at top right and select Extensions. Once you're on the Extensions tab, click Developer Mode, browse to your extension's folder and select it. You'll see the icon appear in your browser toolbar; click it while viewing any normal web page and you should see a popup like the one in the screen shot at the beginning of the article, populated with the data from the current page.

You can [download all the source code here](${cdn2}/files/mab_bookmark_extension.zip) and modify it to suit your own purposes, or just use it to learn from. 

That's it! I'll explain more about Chrome extensions in future posts, but in the meantime, the [Google extension documentation](http://code.google.com/chrome/extensions/docs.html) is comprehensive and very useful to learn from. I also picked up a lot of good information from [this thread on the Chromium Extensions Google Group](http://groups.google.com/group/chromium-extensions/browse_thread/thread/eab847f0a32ec25c/1e1881eea2498a10?lnk=gst&q=update%20popup%20from%20backround%20page#1e1881eea2498a10).