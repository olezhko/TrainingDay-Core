// Generates prerender-routes.txt: the public pages that are rendered to static HTML at build time.
// Blog posts and exercises are read from the production API. If it is unreachable, only static
// pages are prerendered; the rest still works as a normal client-side app.
import { writeFileSync } from 'node:fs';

const API_URL = process.env.PRERENDER_API_URL ?? 'https://api.trainingday.space/api';
const OUTPUT = new URL('../prerender-routes.txt', import.meta.url);

const staticRoutes = ['/', '/exercises', '/blogs', '/about', '/download', '/contact', '/privacy'];

async function fetchJson(url) {
  const response = await fetch(url, { signal: AbortSignal.timeout(30000) });
  if (!response.ok) throw new Error(`${response.status} ${response.statusText} for ${url}`);
  return response.json();
}

async function loadRoutes(name, url, toRoute) {
  try {
    const items = await fetchJson(url);
    return items.map(toRoute);
  } catch (error) {
    console.warn(`[prerender-routes] Skipping ${name}: ${error.message}`);
    return [];
  }
}

const [blogRoutes, exerciseRoutes] = await Promise.all([
  loadRoutes('blogs', `${API_URL}/BlogPosts/search?culture=1&page=1&pageSize=10000`, post => `/blogs/${post.id}`),
  loadRoutes('exercises', `${API_URL}/exercises/search?twoLetterCulture=en`, exercise => `/exercise/details/${exercise.codeNum}`),
]);

const routes = [...staticRoutes, ...blogRoutes, ...exerciseRoutes];
writeFileSync(OUTPUT, routes.join('\n') + '\n');
console.log(`[prerender-routes] ${routes.length} routes (${blogRoutes.length} blogs, ${exerciseRoutes.length} exercises)`);
