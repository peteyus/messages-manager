import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MessageComponent } from './message.component';

describe('MessageComponent', () => {
  let component: MessageComponent;
  let fixture: ComponentFixture<MessageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [],
      declarations: [MessageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(MessageComponent);
    component = fixture.componentInstance;
    component.message = { sender: { displayName: 'Test' }};
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
