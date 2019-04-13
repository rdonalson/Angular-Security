import { AppUserAuth } from './app-user-auth';

export const LOGIN_MOCKS: AppUserAuth[] = [
  {
    userName: 'PSheriff',
    bearerToken: 'abi393kdkd9393ikd',
    isAuthenticated: true,
    canAccessProducts: true,
    canAddProduct: true,
    canSaveProduct: true,
    canAccessCategories: true,
    canAddCategory: false
  },
  {
    userName: 'BJones',
    bearerToken: 'abi393kdkd9393eow',
    isAuthenticated: true,
    canAccessProducts: false,
    canAddProduct: false,
    canSaveProduct: false,
    canAccessCategories: true,
    canAddCategory: true
  }
];
