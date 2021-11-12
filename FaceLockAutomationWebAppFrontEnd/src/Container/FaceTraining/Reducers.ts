import { 
  GET_PERSONFACE_REQUEST, 
  GET_PERSONFACE_SUCCESS, 
  GET_PERSONFACE_FAILED, 
  ADD_FACE_REQUEST,
  ADD_FACE_FAILED,
  ADD_FACE_SUCCESS,
  DELETE_FACE_REQUEST,
  DELETE_FACE_FAILED,
  DELETE_FACE_SUCCESS,
} from './ActionTypes';

const initialState = {
  currentPersonGroupId: 0,
  currentPersonId: 0,
  faces: [],
  isLoading: false,
  error: ''
}

export default function TrainingFaceReducer(state = initialState, action: any) {
  switch (action.type) {
    case GET_PERSONFACE_REQUEST: 
      return {
        ...state,
        isLoading: true,
        currentPersonGroupId: action.personGroupId,
        currentPersonId: action.personId
      }
    case ADD_FACE_REQUEST:
    case DELETE_FACE_REQUEST:
      return {
        ...state,
        isLoading: true
      }
    case GET_PERSONFACE_FAILED:
    case ADD_FACE_FAILED:
    case DELETE_FACE_FAILED:
      return {
        ...state,
        isLoading: false,
        error: action.error
      }
    case GET_PERSONFACE_SUCCESS:
      return {
        ...state,
        isLoading: false,
        faces: action.response
      }
    case ADD_FACE_SUCCESS:
      const newFace = action.faces[0];
      const shouldInclude = state.currentPersonId === newFace.personId;
      return Object.assign({}, state, {
        isLoading: false,
        faces: shouldInclude ? [...state.faces, ...action.faces] : [ ...state.faces ]
      })
    case DELETE_FACE_SUCCESS:
      const remainFaces = state.faces.filter(fc => fc.id !== action.delFaceId)
      return {
        ...state,
        isLoading: false,
        faces: remainFaces
      }
    default:
      return state;
  }
}