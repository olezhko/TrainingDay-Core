import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ExercisePreview } from 'src/app/data/exercises/exercise-preview.model';
import { ExerciseEditParams } from 'src/app/data/exercises/exercise-params.model';
import { catchError } from 'rxjs/operators';
import { BlogPostEditViewModel, BlogPreview } from '../../data/blog/blog-preview.model';
import { BlogDetails } from '../../data/blog/blog-details.model';
import { environment } from '../../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class BackendService {

  private baseApiUrl = environment.baseApiUrl;

  private exerciseApiUrl = this.baseApiUrl + '/exercises';
  private exerciseSearchUrl = this.baseApiUrl + '/exercises/search';
  private exerciseEditorUrl = this.baseApiUrl + '/exercises/editor';

  private baseUrl = this.baseApiUrl + '/BlogPosts';

  constructor(private http: HttpClient) {}

  sendMessage(name: string, email: string, message: string): Observable<any> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Access-Control-Allow-Origin': environment.baseUrl,
        'Access-Control-Allow-Credentials': 'true'
      })
    };

    const data = { name, email, message };
    return this.http.post<any>(environment.baseUrl + '/support/contact-me', data, httpOptions);
  }

  getExercises(selectedMuscle: number | undefined, filterName: string, twoLetterCulture: string): Observable<ExercisePreview[]> {
    if (selectedMuscle === undefined)
      return this.http.get<ExercisePreview[]>(`${this.exerciseSearchUrl}?twoLetterCulture=${twoLetterCulture}&filterName=${filterName}`);
    
    return this.http.get<ExercisePreview[]>(`${this.exerciseSearchUrl}?twoLetterCulture=${twoLetterCulture}&filterName=${filterName}&selectedMuscle=${selectedMuscle}`);
  }

  getExerciseEditParams(twoLetterCulture: string) : Observable<ExerciseEditParams> {
    return this.http.get<ExerciseEditParams>(`${this.exerciseEditorUrl}?cu=${twoLetterCulture}`);
  }

  editExercise(id: number, exerciseViewModel: any): Observable<any> {
    const url = `${this.exerciseApiUrl}/${id}`;
    return this.http.put(url, exerciseViewModel)
      .pipe(
        catchError(this.handleError)
      );
  }

  deleteExercise(id: number): Observable<any> {
    const url = `${this.exerciseApiUrl}/${id}`;
    return this.http.delete(url)
      .pipe(
        catchError(this.handleError)
      );
  }

  getExerciseDetails(codeNum: number, twoLetterCulture: string): Observable<any> {
    const url = `${this.exerciseApiUrl}?codeNum=${codeNum}&twoLetterCulture=${twoLetterCulture}`;
    return this.http.get(url)
      .pipe(
        catchError(this.handleError)
      );
  }

  createExercise(model: any): Observable<any> {
    return this.http.post(this.exerciseApiUrl, model)
      .pipe(
        catchError(this.handleError)
      );
  }

  getBlogPosts(culture: string, page: number, pageSize: number): Observable<BlogPreview[]> {
    let params = new HttpParams()
      .set('culture', 1)
      .set('page', page)
      .set('pageSize', pageSize);
    return this.http.get<BlogPreview[]>(`${this.baseUrl}/search`, { params });
  }

  getBlogPost(id: number): Observable<BlogPostEditViewModel> {
    return this.http.get<BlogPostEditViewModel>(`${this.baseUrl}?id=${id}`);
  }

  createBlogPost(blogPost: BlogPostEditViewModel): Observable<BlogDetails> {
    return this.http.post<BlogPostEditViewModel>(this.baseUrl, blogPost);
  }

  editBlogPost(id: number, blogPost: BlogPostEditViewModel): Observable<BlogDetails> {
    return this.http.put<BlogDetails>(`${this.baseUrl}?id=${id}`, blogPost);
  }

  deleteBlogPost(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}?id=${id}`);
  }

  private handleError(error: any): Promise<any> {
    console.error('An error occurred', error);
    return Promise.reject(error.message || error);
  }
}
