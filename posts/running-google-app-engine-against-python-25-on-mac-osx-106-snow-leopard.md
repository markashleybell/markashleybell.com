Title: Running Google App Engine against Python 2.5 on Mac OSX 10.6 Snow Leopard
Abstract: How to point the Google App Engine Launcher to the supported Python version on Mac OSX 10.6 Snow Leopard.
Date: 2010-03-15 03:01

I ran into a strange error yesterday night while working on an App Engine project in Python. Most of the application worked perfectly, but certain views threw errors like the following:

`ImportError: No module named _ctypes`

However, the same application worked perfectly on my Windows 7 development server.

I was puzzled for a while, but it turns out that [App Engine currently only supports Python 2.5.x](http://code.google.com/p/googleappengine/issues/detail?id=757), while in Snow Leopard the default Python version was updated to 2.6.1.

Python 2.5 is still installed on Snow Leopard, so it's just a case of explicitly pointing the App Engine development server at the correct version. If you're using the Google App Engine Launcher, open the Preferences dialog, and enter the following into the `Python Path` field:

`/usr/local/bin/python`

_Edit: as Ruchi points out in the comments, make sure you press Enter after inputting the path or the change will not take effect._

The development server will now run your apps against the correct (supported) version of Python and the errors should disappear.