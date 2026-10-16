# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

# TrainingDay-Core

Backend + website for the TrainingDay fitness app: an ASP.NET Core (.NET 10) API used by the mobile app
and by an Angular 17 site (trainingday.space) that is served from the same server.

## Commands

```bash
# Backend (from repo root)
dotnet build TrainingDay.Web.sln
dotnet run --project TrainingDay.Web.Server --launch-profile https   # https://localhost:7081, Swagger at /swagger

# Tests (xUnit)
dotnet test TrainingDay.Web.Services.Tests
dotnet test TrainingDay.Web.Services.Tests --filter "FullyQualifiedName~DateTimeExtensionsTests"

# EF Core migrations (DbContext lives in Database, startup project is Server)
dotnet ef migrations add <Name> --project TrainingDay.Web.Database --startup-project TrainingDay.Web.Server

# Frontend (in trainingday.web.client)
npm start          # ng serve over https on 127.0.0.1:4200 (dev cert via aspnetcore-https.js)
npm run build      # production build with prerendering — see "Build and verify" below
npm test           # Karma/Jasmine
```

`TrainingDay.Common` also targets `net10.0-ios`/`net10.0-android`, which need MAUI workloads. On machines without
them (e.g. CI), restore/publish the server with `-p:TargetFramework=net10.0` as `.github/workflows/aws.yml` does.

## Architecture

Project dependency chain (top depends on bottom):

- `TrainingDay.Web.Server`: controllers, view models, hand-written mapping extensions in `Mapper/` (no AutoMapper), startup. `Extensions/DependenciesInjection.cs`
  (`InstallServices`) registers every service, Identity, options sections and Firebase. Register new services there.
- `TrainingDay.Web.Services`: business logic as `I*Manager`/`*Manager` pairs, one folder per feature (Blogs, Exercises,
  SocialWorkouts, UserTokens, Auth/JWT, Email via MailKit, Firebase push, OpenAI, RabbitMQ producer, YouTube).
  Background work runs through `ConsumeScopedServiceHostedService` → `IScopedProcessingService`.
- `TrainingDay.Web.Database`: `TrainingDayContext` (EF Core, MySQL via `MySql.EntityFrameworkCore`, ASP.NET Identity
  with `MobileUser` / `IdentityRole<Guid>`), migrations, `AuditSaveChangesInterceptor`, `ExercisesInitializer` seed data.
- `TrainingDay.Web.Data`: DTOs/view models and repository interfaces shared by services.
- `TrainingDay.Web.Entities`: EF entity classes.
- `TrainingDay.Common`: shared models/resources, multi-targeted (`net10.0`, `-ios`, `-android`) because the mobile app uses it too.
  Keep it free of server-only dependencies.

Startup behavior (`Program.cs`): pending EF migrations are applied automatically on launch, exercises are seeded,
and the `Admin`/`User` roles are created. Firebase is initialized from `TrainingDay.Web.Server/Settings/firebase.json`
(the app won't start without it). Connection string `DefaultConnection`, plus `JwtSettings`, `EmailSettings`,
`OpenAI`, `ApiSettings` config sections.

Auth: two schemes are combined in the default policy — Identity cookies (website) and JWT bearer (mobile app).
For `/api/*` paths the cookie handler returns 401/403 instead of redirecting. Rate limiting is configured in
`Extensions/RateLimitingExtensions.cs`.

Frontend (`trainingday.web.client`): Angular 17 standalone components, NgRx (exercise store in `services/ngrx`),
Angular Material + ng-bootstrap. All HTTP goes through `services/backend/backend.service.ts` using
`environment.baseApiUrl` (`src/environment/`: dev → `https://localhost:7081/api`, prod → `https://api.trainingday.space/api`).
In development the server's SpaProxy launches `npm start`; in production the built `browser/` output is served as
static files from the server, with `SpaFallbackExtensions` mapping client routes.

Deployment: pushes to `master` trigger `.github/workflows/aws.yml`, which publishes the server and restarts a
systemd service on an EC2 host.

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
