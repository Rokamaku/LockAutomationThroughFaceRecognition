import * as React from 'react';
import { Provider } from "react-redux";
import { BrowserRouter, Router } from 'react-router-dom'
import App from './App';
import createBrowserHistory from 'history/createBrowserHistory';

const customHistory = createBrowserHistory();

export const Root = ({ store }) => (
  <Provider store={store} >
    <BrowserRouter >
      <Router history={customHistory}>
        <App />
      </Router>
    </BrowserRouter>
  </Provider>
)