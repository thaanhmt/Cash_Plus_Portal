import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommentProductComponent } from './comment-product.component';

describe('CommentProductComponent', () => {
  let component: CommentProductComponent;
  let fixture: ComponentFixture<CommentProductComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommentProductComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommentProductComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
