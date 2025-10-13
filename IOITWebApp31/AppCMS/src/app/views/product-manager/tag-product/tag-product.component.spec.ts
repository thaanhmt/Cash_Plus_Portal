import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TagProductComponent } from './tag-product.component';

describe('TagProductComponent', () => {
  let component: TagProductComponent;
  let fixture: ComponentFixture<TagProductComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TagProductComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TagProductComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
