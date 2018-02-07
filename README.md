# Static site generator for markb.co.uk

## Prerequisites

- Python 3

## Development helpers

Open a shell at the project root. The following commands are available (currently Windows only):

- `dev env`  
  Starts the Python `venv` environment
- `publish`  
   Generate site HTML files in `/public`; will only work within the environment set up by `dev env`
- `dev server`  
  Start a web server pointing at the `public` folder (port is defined in `tools\https-dev-server.py`)
- `dev gen-css [style-name]`  
  Generate a stylesheet named `[style-name].css` in `css\vendor`, which contains CSS rules for the corresponding Pygments style
- `dev list-styles`  
  List all built-in Pygments styles

## Bootstrap components

### Common CSS

* Grid system
* Tables

### Utilities

* Responsive utilities
