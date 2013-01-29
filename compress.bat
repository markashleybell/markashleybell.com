cscript //nologo "%1\lessc.wsf" %1\markashleybell.com\Content\Less\Site.less %1\markashleybell.com\Content\Css\Site.css

java -jar %1\yuicompressor-2.4.7.jar -o %1\markashleybell.com\Content\Css\Site.min.css %1\markashleybell.com\Content\Css\Site.css

copy /b %1\markashleybell.com\Scripts\twitter.js+%1\markashleybell.com\Scripts\github.js+%1\markashleybell.com\Scripts\email.js+%1\markashleybell.com\Scripts\analytics.js %1\markashleybell.com\Scripts\script.js

java -jar %1\yuicompressor-2.4.7.jar -o %1\markashleybell.com\Scripts\script.min.js %1\markashleybell.com\Scripts\script.js