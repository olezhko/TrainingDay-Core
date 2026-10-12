// After prerendering, browser/index.html contains the rendered home page. Pages that are not
// prerendered (login, profile, blog posts published after the build...) need an empty shell instead,
// otherwise Angular would try to hydrate them against the home page markup.
// Creates browser/index.csr.html, which the ASP.NET server uses as the SPA fallback.
import { readFileSync, writeFileSync } from 'node:fs';
import { join } from 'node:path';

const browserDir = process.argv[2] ?? 'dist/trainingday.web.client/browser';
const html = readFileSync(join(browserDir, 'index.html'), 'utf8');

const shell = html
  .replace(/<app-root[^>]*>[\s\S]*<\/app-root>/, '<app-root></app-root>')
  .replace(/<script id="ng-state"[^>]*>[\s\S]*?<\/script>/, '')
  .replace(/<style ng-app-id="[^"]*">[\s\S]*?<\/style>/g, '')
  .replace('<!--nghm-->', '');

if (!shell.includes('<app-root></app-root>')) {
  throw new Error(`[csr-shell] <app-root> not found in ${browserDir}/index.html`);
}

writeFileSync(join(browserDir, 'index.csr.html'), shell);
console.log(`[csr-shell] Created ${join(browserDir, 'index.csr.html')}`);
