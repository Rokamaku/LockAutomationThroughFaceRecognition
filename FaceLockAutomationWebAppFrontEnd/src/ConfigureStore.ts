import { applyMiddleware, createStore, Store, AnyAction } from 'redux';
import { composeWithDevTools } from 'redux-devtools-extension';
import { createEpicMiddleware } from 'redux-observable';
import { rootEpic, rootReducer } from './RootReducer';



function configureStore() : Store<any, AnyAction> {
  const epicMiddleware = createEpicMiddleware();
  
  const store = createStore(
    rootReducer,
    undefined,
    composeWithDevTools(applyMiddleware(epicMiddleware))
  );

  epicMiddleware.run(rootEpic);

  return store;
}

export const storeConf = configureStore();