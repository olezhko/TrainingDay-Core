import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { LandingPageComponent } from "./landing-page/landing-page.component";
import { ContactMeComponent } from "./contact-me/contact-me.component";
import { PrivacyComponent } from "./privacy/privacy.component";
import { AboutComponent } from "./about/about.component";
import { NotFoundComponent } from "./not-found/not-found.component";
import { ExerciseListComponent } from './body-components/exercise-list/exercise-list.component';
import { BlogListComponent } from './body-components/blog-list/blog-list.component';
import { BlogItemComponent } from './body-components/blog-item/blog-item.component';
import { ExerciseItemComponent } from './body-components/exercise-item/exercise-item.component';
import { ExerciseEditComponent } from './body-components/exercise-edit/exercise-edit.component';

// определение маршрутов
const routes: Routes = [
  { path: "", component: LandingPageComponent },
  { path: "blogs", component: BlogListComponent },
  { path: "blog/:id", component: BlogItemComponent },
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