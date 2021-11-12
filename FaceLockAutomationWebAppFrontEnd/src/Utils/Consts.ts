export const signalrInfoUrl = 'http://localhost:7071/api/getSignalRInfo';

export const urlPaths = {
  Root: "/",
  Home: '/home',
  ActivityLog: '/activityLog',
  FaceTraining: '/faceTraining',
  DeviceControl: '/deviceControl'
}

export const dropdown = {
  intializeNum: 0,
  none: -1,
  all: -2
}

export const sortTypeKey = {
  ascending: 'asc',
  descending: 'desc'
}

export const faceLogType = {
  allFaceLog: 1,
  knownFaceLog: 2,
  unknownFaceLog: 3,
  undetectedFaceLog: 4,
  personFaceLog: 5
}

export const addingTrainingType = {
  personGroup: 1,
  person: 2,
  face: 3
}

export const TrainingStatusName = [
  'Succeeded', 
  'Failed', 
  'Running', 
  'Not Train', 
]