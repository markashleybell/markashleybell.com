@echo off
pygmentize -f html -S %1 -a .codehilite > css/vendor/%1.css
