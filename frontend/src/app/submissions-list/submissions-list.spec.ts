import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubmissionsList } from './submissions-list';

describe('SubmissionsList', () => {
  let component: SubmissionsList;
  let fixture: ComponentFixture<SubmissionsList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SubmissionsList],
    }).compileComponents();

    fixture = TestBed.createComponent(SubmissionsList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
