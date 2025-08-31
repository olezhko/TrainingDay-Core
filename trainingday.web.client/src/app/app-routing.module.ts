import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { LandingPageComponent } from "./components/landing-page/landing-page.component";
import { ContactMeComponent } from "./components/contact-me/contact-me.component";
import { PrivacyComponent } from "./components/privacy/privacy.component";
import { AboutComponent } from "./components/about/about.component";
import { NotFoundComponent } from "./components/not-found/not-found.component";
import { ExerciseListComponent } from './components/body-components/exercise-list/exercise-list.component';
import { BlogListComponent } from './components/body-components/blog-list/blog-list.component';
import { BlogItemComponent } from './components/body-components/blog-item/blog-item.component';
import { ExerciseItemComponent } from './components/body-components/exercise-item/exercise-item.component';
import { ExerciseEditComponent } from './components/body-components/exercise-edit/exercise-edit.component';
import { BlogEditComponent } from './components/body-components/blog-edit/blog-edit.component';

// определение маршрутов
const routes: Routes = [
  { path: "", component: LandingPageComponent },
  { path: "blogs", component: BlogListComponent },
  { path: "blogs/new", component: BlogEditComponent },   // exact match first
  { path: "blogs/edit/:id", component: BlogEditComponent }, 
  { path: "blogs/:id", component: BlogItemComponent },   // generic last
  { path: "exercises", component: ExerciseListComponent },
  { path: "exercise/details/:id", component: ExerciseItemComponent },
  { path: "exercise/edit/:id", component: ExerciseEditComponent },
  { path: "exercise/new", component: ExerciseEditComponent },
  { path: "contact", component: ContactMeComponent },
  { path: "privacy", component: PrivacyComponent },
  { path: "about", component: AboutComponent },
  { path: "**", component: NotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingComponent {
}
