import { bootstrapApplication } from '@angular/platform-browser';

import { AppComponent } from './app/app.component';
import { serverConfig } from './app/app.config.server';

/** Used only at build time to prerender public pages to static HTML. */
const bootstrap = () => bootstrapApplication(AppComponent, serverConfig);

export default bootstrap;
