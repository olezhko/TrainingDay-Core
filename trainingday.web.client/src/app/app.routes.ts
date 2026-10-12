import { Routes } from '@angular/router';
import { LandingPageComponent } from './components/landing-page/landing-page.component';
import { ContactMeComponent } from './components/contact-me/contact-me.component';
import { PrivacyComponent } from './components/privacy/privacy.component';
import { AboutComponent } from './components/about/about.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { ExerciseListComponent } from './components/body-components/exercise-list/exercise-list.component';
import { BlogListComponent } from './components/body-components/blog-list/blog-list.component';
import { BlogItemComponent } from './components/body-components/blog-item/blog-item.component';
import { ExerciseItemComponent } from './components/body-components/exercise-item/exercise-item.component';
import { ExerciseEditComponent } from './components/body-components/exercise-edit/exercise-edit.component';
import { BlogEditComponent } from './components/body-components/blog-edit/blog-edit.component';
import { DownloadPageComponent } from './components/download-page/download-page.component';
import { LoginComponent } from './components/auth/login/login.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { ForgotPasswordComponent } from './components/auth/forgot-password/forgot-password.component';
import { UserProfileComponent } from './components/auth/user-profile/user-profile.component';
import { ConfirmEmailComponent } from './components/auth/confirm-email/confirm-email.component';
import { authGuard } from './guards/auth.guard';

const noindex = { seo: { noindex: true } };

export const routes: Routes = [
  { path: '', component: LandingPageComponent, data: { seo: {} } },
  { path: 'login', component: LoginComponent, data: { seo: { title: 'Sign In', noindex: true } } },
  { path: 'register', component: RegisterComponent, data: { seo: { title: 'Register', noindex: true } } },
  { path: 'forgot-password', component: ForgotPasswordComponent, data: noindex },
  { path: 'confirm-email', component: ConfirmEmailComponent, data: noindex },
  { path: 'profile', component: UserProfileComponent, canActivate: [authGuard], data: noindex },
  { path: 'blogs', component: BlogListComponent, data: { seo: {
    title: 'Fitness Blog – Workout Tips & Nutrition Guides',
    description: 'Expert fitness advice, workout tips, training programs and nutrition guides from the TrainingDay team.' } } },
  { path: 'blogs/new', component: BlogEditComponent, data: noindex },
  { path: 'blogs/edit/:id', component: BlogEditComponent, data: noindex },
  { path: 'blogs/:id', component: BlogItemComponent, data: { seo: { title: 'Blog', type: 'article' } } },
  { path: 'exercises', component: ExerciseListComponent, data: { seo: {
    title: 'Exercise Library – Technique Guides & Target Muscles',
    description: 'Browse the complete TrainingDay exercise library: step-by-step technique, starting position, target muscles and difficulty for every exercise.' } } },
  { path: 'exercise/details/:id', component: ExerciseItemComponent, data: { seo: { title: 'Exercise' } } },
  { path: 'exercise/edit/:id', component: ExerciseEditComponent, data: noindex },
  { path: 'exercise/new', component: ExerciseEditComponent, data: noindex },
  { path: 'contact', component: ContactMeComponent, data: { seo: { title: 'Contact Us', description: 'Get in touch with the TrainingDay team: questions, feedback and support for the TrainingDay workout app.' } } },
  { path: 'privacy', component: PrivacyComponent, data: { seo: { title: 'Privacy Policy', description: 'Privacy policy of the TrainingDay workout tracker app and website.' } } },
  { path: 'about', component: AboutComponent, data: { seo: { title: 'About TrainingDay', description: 'Learn about TrainingDay – a workout tracker app to plan trainings, log sets and follow your progress on Android and iOS.' } } },
  { path: 'download', component: DownloadPageComponent, data: { seo: { title: 'Download the App', description: 'Download TrainingDay workout tracker for free on Google Play and the App Store.' } } },
  { path: '**', component: NotFoundComponent, data: { seo: { title: 'Page Not Found', noindex: true } } }
];
