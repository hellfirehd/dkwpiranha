# dkw.io

## Quick Start

Run these commands from this directory:

```bash
npm install
npm run build:css
dotnet run
```

## Frontend CSS

- One-time build: `npm run build:css`
- Watch mode while editing SCSS: `npm run watch:css`

SCSS source: `assets/scss/style.scss`  
Output CSS: `wwwroot/assets/css/style.css` and `wwwroot/assets/css/style.min.css`

## Docker Deployment

For deployment workflow and troubleshooting, see: [Deployment](docs/DEPLOYMENT_DKWIO.md)