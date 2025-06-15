import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { AdminComponent } from './features/profile/profile.component';
import { LoginComponent } from './features/login/login.component';
import { SignupComponent } from './features/signup/signup.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { AuthGuard } from './core/guards/auth.guard';
import { CartComponent } from './features/cart/cart.component';
import { OrderComponent } from './features/order/order.component';
import { DetailsComponent } from './features/details/details.component';
import { AdminProductsComponent } from './features/admin-products/admin-products.component';
import { AdminGuard } from './core/guards/admin.guard';
import { AdminOrdersComponent } from './features/admin-orders/admin-orders.component';
import { AdminCustomersComponent } from './features/admin-customers/admin-customers.component';
import { AdminCategoriesComponent } from './features/admin-categories/admin-categories.component';
import { NotFoundComponent } from './features/not-found/not-found.component';
import { CompleteOrderComponent } from './features/complete-order/complete-order.component';
import { PaymentComponent } from './features/payment/payment.component';
import { NotPaidComponent } from './features/not-paid/not-paid.component';
import { UserAgreementComponent } from './features/user-agreement/user-agreement.component';
import { PrivacyPolicyComponent } from './features/privacy-policy/privacy-policy.component';
import { FaqComponent } from './features/faq/faq.component';
import { AboutComponent } from './features/about/about.component';


export const routes: Routes = [
  { path: "", component: HomeComponent },
  { path: "profile", component: AdminComponent, canActivate: [AuthGuard]},
  { path: "login", component: LoginComponent},
  { path: "signup", component: SignupComponent},
  { path: "cart", component: CartComponent},
  { path: "order", component: OrderComponent},
  { path: "payment", component: PaymentComponent},
  { path: "complete", component: CompleteOrderComponent},
  { path: "not-paid", component: NotPaidComponent},
  { path: "user-agreement", component: UserAgreementComponent},
  { path: "privacy-policy", component: PrivacyPolicyComponent},
  { path: "faq", component: FaqComponent},
  { path: "about", component: AboutComponent},
  { path: "admin-orders", component: AdminOrdersComponent, canActivate: [AdminGuard]},
  { path: "admin-customers", component: AdminCustomersComponent, canActivate: [AdminGuard]},
  { path: "admin-categories", component: AdminCategoriesComponent, canActivate: [AdminGuard]},
  { path: "admin-products", component: AdminProductsComponent, canActivate: [AdminGuard]},
  { path: "details/:id", component: DetailsComponent, canActivate: [AuthGuard]},
  { path: "dashboard", component: DashboardComponent, canActivate: [AdminGuard]},
  { path: "**", component: NotFoundComponent}
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,

  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
