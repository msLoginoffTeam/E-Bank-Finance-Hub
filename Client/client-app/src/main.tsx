import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import {Provider} from "react-redux";
import {store} from "./store/store.ts";
import './global.css';
import {QueryClientProvider} from "@tanstack/react-query";
import {queryClient} from "./lib/reactQueryClient.ts";

createRoot(document.getElementById('root')!).render(
  <StrictMode>
      <QueryClientProvider client={queryClient}>
      <Provider store={store}>
          <App />
      </Provider>
      </QueryClientProvider>
  </StrictMode>,
)
