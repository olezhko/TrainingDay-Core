# TrainingDay-Core

## Website (trainingday.space) — SEO and prerendering rules

The public site is the Angular app in `trainingday.web.client`, served by `TrainingDay.Web.Server` (ASP.NET Core).
Production domain: `https://trainingday.space`, API: `https://api.trainingday.space`.
Public pages are **prerendered to static HTML at build time** so Google indexes them. Keep it working:

### Adding or changing a route (`src/app/app.routes.ts`)
- Every route needs `data: { seo: {...} }` — `title` + `description` for public pages, `noindex: true` for
  auth/admin/editor pages. `SeoService` (`src/app/services/seo/seo.service.ts`) applies it on navigation.
- Update the route list in `TrainingDay.Web.Server/Extensions/SpaFallbackExtensions.cs`, otherwise the server returns 404.
- Public page: add it to `staticRoutes` in `scripts/prerender-routes.mjs` and `StaticPages` in
  `TrainingDay.Web.Server/Controllers/SitemapController.cs`.
- Private page: add a `Disallow` line to `src/robots.txt` if crawlers should not visit it.

### Pages that load content from the API
- After the data loads, call `seoService.update({ title, description, image?, type? })` (see blog-item, exercise-item).
- New dynamic page types (like `/blogs/:id`) must also be added to `scripts/prerender-routes.mjs` and the sitemap.

### Code must run at build time (prerender runs it in Node, without a browser)
- Don't use `window`, `document`, `navigator`, `localStorage` directly in code that runs while rendering
  (constructors, `ngOnInit`, services). Guard with `isPlatformBrowser(inject(PLATFORM_ID))` or
  `typeof navigator !== 'undefined'`, or use `inject(DOCUMENT)`.
- No `setInterval` / endless timers on public pages without a browser guard — they block prerendering.
- Client hydration is enabled (`provideClientHydration`): avoid manual DOM manipulation; use `ngSkipHydration` if unavoidable.
- Global CSS: no webpack `~` imports (the esbuild `application` builder doesn't support them).

### Links and crawlability
- Navigation must be real links: `<a routerLink="...">`, never `(click)` + `router.navigate` for content
  pages — Google only follows `<a href>`.
- Images: always meaningful `alt`; use `loading="lazy"` below the fold.
- `/download` must not redirect crawlers (bot user agents are skipped).

### Build and verify
- `npm run build` in `trainingday.web.client` runs `prebuild` (route list from the API) → `ng build`
  (prerender) → `postbuild` (`index.csr.html` shell). Output: `dist/trainingday.web.client/browser/`.
  Don't call `ng build` alone for production — the route list and shell won't be generated.
- After changes to public pages, run the production build and check the prerendered HTML
  (e.g. `browser/blogs/<id>/index.html`) contains the title, description, canonical and content.
- `ng serve` uses the `development` configuration with prerendering off, so SSR-only problems show up
  only in the production build.
