import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubmitCard } from './submit-card';

describe('SubmitCard', () => {
  let component: SubmitCard;
  let fixture: ComponentFixture<SubmitCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SubmitCard],
    }).compileComponents();

    fixture = TestBed.createComponent(SubmitCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
