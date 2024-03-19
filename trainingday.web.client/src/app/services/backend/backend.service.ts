import { Injectable } from '@angular/core';
import { HttpClient , HttpHeaders} from '@angular/common/http';
import { Observable } from 'rxjs';
import { ExercisePreview } from 'src/app/data/exercises/exercise-preview.model';
import { ExerciseEditParams } from 'src/app/data/exercises/exercise-params.model';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class BackendService {
  private exerciseSearchUrl = 'https://localhost:7081/api/exercises/search';
  private exerciseEditorUrl = 'https://localhost:7081/api/exercises/editor';
  private exerciseApiUrl = 'https://localhost:7081/api/exercises';

  constructor(private http: HttpClient) {}

  sendMessage(name: string, email: string, message: string): Observable<any> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Access-Control-Allow-Origin': 'http://localhost:7081',
        'Access-Control-Allow-Credentials': 'true'
      })
    };

    const data = { name, email, message };
    return this.http.post<any>('https://localhost:7081/support/contact-me', data, httpOptions);
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
    const url = `${this.exerciseApiUrl}/details?codeNum=${codeNum}&twoLetterCulture=${twoLetterCulture}`;
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

  private handleError(error: any): Promise<any> {
    console.error('An error occurred', error);
    return Promise.reject(error.message || error);
  }
}