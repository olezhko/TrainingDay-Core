import { HttpClientModule } from '@angular/common/http';
import { NgModule, isDevMode } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { HeaderComponent } from './components/header/header.component';
import { FooterComponent } from './components/footer/footer.component';
import { ExerciseListComponent } from './components/body-components/exercise-list/exercise-list.component';
import { BlogListComponent } from './components/body-components/blog-list/blog-list.component';
import { BlogItemComponent } from './components/body-components/blog-item/blog-item.component';
import { BlogEditComponent } from './components/body-components/blog-edit/blog-edit.component';
import { ExerciseItemComponent } from './components/body-components/exercise-item/exercise-item.component';
import { RouterOutlet } from "@angular/router";
import { LandingPageComponent } from './components/landing-page/landing-page.component';
import { ContactMeComponent } from './components/contact-me/contact-me.component';
import { PrivacyComponent } from './components/privacy/privacy.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { AppRoutingComponent } from './app-routing.module';
import { JoinUsComponent } from './components/landing-page/join-us/join-us.component';
import { AboutUsComponent } from './components/landing-page/about-us/about-us.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ExercisePreviewComponent } from './components/body-components/exercise-preview/exercise-preview.component';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { ExerciseEditComponent } from './components/body-components/exercise-edit/exercise-edit.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from './material.module';
import { NgxEditorModule } from 'ngx-editor';
import { MatSnackBarModule } from '@angular/material/snack-bar';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent,
    ExerciseListComponent,
    BlogListComponent,
    BlogItemComponent,
    BlogEditComponent,
    ExerciseItemComponent,
    LandingPageComponent, JoinUsComponent, AboutUsComponent,
    ContactMeComponent,
    PrivacyComponent,
    NotFoundComponent,
    ExercisePreviewComponent,
    ExerciseEditComponent,
  ],
  imports: [
    MatSnackBarModule, NgxEditorModule, BrowserModule, MaterialModule, RouterOutlet, HttpClientModule, AppRoutingComponent, FormsModule, ReactiveFormsModule, NgbModule, StoreModule.forRoot({}, {}), EffectsModule.forRoot([]), StoreDevtoolsModule.instrument({ maxAge: 25, logOnly: !isDevMode() }), BrowserAnimationsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
