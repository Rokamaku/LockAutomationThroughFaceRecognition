import { combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';
import activityLogReducer from './Container/ActivityLog/Reducers';
import trainingFaceReducer from './Container/FaceTraining/Reducers';
import growlReducer from './Growl/Reducers';
import commonReducer from './Common/Reducers';
import { 
  getFaceLogInGroupEpic, 
  getPersonFaceLogEpic, 
  getKnownFaceLogEpic,
  getUnknownFaceLogEpic,
  getUndetectedFaceLogEpic
} from './Container/ActivityLog/Actions';
import { 
  getAllPersonGroupEpic, 
  addPersonGroupEpic, 
  deletePersonGroupEpic, 
  addPersonEpic,
  deletePersonEpic,
  changeDefaultGroupEpic
} from './Common/Action';
import { 
  getPersonFaceEpic, 
  addFaceEpic, 
  deleteFaceEpic
} from './Container/FaceTraining/Actions';

export const rootEpic = combineEpics(
  getFaceLogInGroupEpic,
  getPersonFaceLogEpic,
  getKnownFaceLogEpic,
  getUnknownFaceLogEpic,
  getUndetectedFaceLogEpic,
  getAllPersonGroupEpic,
  getPersonFaceEpic,
  addPersonGroupEpic,
  deletePersonGroupEpic,
  changeDefaultGroupEpic,
  addPersonEpic,
  deletePersonEpic,
  addFaceEpic,
  deleteFaceEpic
);
export const rootReducer = combineReducers({
  activityLog: activityLogReducer,
  trainingFace: trainingFaceReducer,
  common: commonReducer,
  growl: growlReducer
});