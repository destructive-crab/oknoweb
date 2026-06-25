import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubmitSuccess } from './submit-success';

describe('SubmitSuccess', () => {
  let component: SubmitSuccess;
  let fixture: ComponentFixture<SubmitSuccess>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SubmitSuccess],
    }).compileComponents();

    fixture = TestBed.createComponent(SubmitSuccess);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
