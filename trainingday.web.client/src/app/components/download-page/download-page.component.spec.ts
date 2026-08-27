import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DownloadPageComponent } from './download-page.component';

describe('DownloadPageComponent', () => {
  let component: DownloadPageComponent;
  let fixture: ComponentFixture<DownloadPageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [DownloadPageComponent]
    });
    fixture = TestBed.createComponent(DownloadPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
