/** Content languages the API has data for. */
export type ContentCulture = 'ru' | 'en';

export const DEFAULT_CONTENT_CULTURE: ContentCulture = 'en';

/** Maps any language code (e.g. "ru-RU", "de") to a culture the API supports; unknown ones fall back to English. */
export function toContentCulture(language: string | null | undefined): ContentCulture {
  return language?.toLowerCase().split('-')[0] === 'ru' ? 'ru' : DEFAULT_CONTENT_CULTURE;
}

/** The visitor's content culture. navigator is not available while prerendering, so the build uses English. */
export function getContentCulture(): ContentCulture {
  return toContentCulture(typeof navigator !== 'undefined' ? navigator.language : null);
}

/**
 * Blog culture id for /BlogPosts/search. Production blog posts are stored with 1 = English, 2 = Russian,
 * the reverse of the Cultures table (1 = ru, 2 = en); scripts/prerender-routes.mjs relies on the same mapping.
 */
export function toBlogCultureId(culture: ContentCulture): number {
  return culture === 'en' ? 1 : 2;
}
