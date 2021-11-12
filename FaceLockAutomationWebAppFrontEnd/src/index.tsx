import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { storeConf } from './ConfigureStore';
import registerServiceWorker from './registerServiceWorker';
import { Root } from './Root';


ReactDOM.render(
  <Root store={storeConf} />,
  document.getElementById('root') as HTMLElement
);
registerServiceWorker();
