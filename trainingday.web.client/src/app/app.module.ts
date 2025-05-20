import { HttpClientModule } from '@angular/common/http';
import { NgModule, isDevMode } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { ExerciseListComponent } from './body-components/exercise-list/exercise-list.component';
import { BlogListComponent } from './body-components/blog-list/blog-list.component';
import { BlogItemComponent } from './body-components/blog-item/blog-item.component';
import { ExerciseItemComponent } from './body-components/exercise-item/exercise-item.component';
import { RouterOutlet } from "@angular/router";
import { LandingPageComponent } from './landing-page/landing-page.component';
import { ContactMeComponent } from './contact-me/contact-me.component';
import { PrivacyComponent } from './privacy/privacy.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { AppRoutingComponent } from './app-routing.module';
import { JoinUsComponent } from './landing-page/join-us/join-us.component';
import { AboutUsComponent } from './landing-page/about-us/about-us.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ExercisePreviewComponent } from './body-components/exercise-preview/exercise-preview.component';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { BackendService } from './services/backend/backend.service';
import { ExerciseEditComponent } from './body-components/exercise-edit/exercise-edit.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from './material.module';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent,
    ExerciseListComponent,
    BlogListComponent,
    BlogItemComponent,
    ExerciseItemComponent,
    LandingPageComponent, JoinUsComponent, AboutUsComponent,
    ContactMeComponent,
    PrivacyComponent,
    NotFoundComponent,
    ExercisePreviewComponent,
    ExerciseEditComponent,
  ],
  imports: [
    BrowserModule, MaterialModule, RouterOutlet, HttpClientModule, AppRoutingComponent, FormsModule, ReactiveFormsModule, NgbModule, StoreModule.forRoot({}, {}), EffectsModule.forRoot([]), StoreDevtoolsModule.instrument({ maxAge: 25, logOnly: !isDevMode() }), BrowserAnimationsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
