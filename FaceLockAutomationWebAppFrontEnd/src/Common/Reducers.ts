import { 
  GET_ALL_PERSONGROUP_REQUEST, 
  GET_ALL_PERSONGROUP_SUCCESS, 
  GET_ALL_PERSONGROUP_FAILED, 
  ADD_PERSONGROUP_REQUEST,
  ADD_PERSONGROUP_SUCCESS,
  ADD_PERSONGROUP_FAILED,
  DELETE_PERSONGROUP_SUCCESS,
  DELETE_PERSONGROUP_FAILED,
  DELETE_PERSONGROUP_REQUEST,
  ADD_PERSON_REQUEST,
  ADD_PERSON_FAILED,
  ADD_PERSON_SUCCESS,
  DELETE_PERSON_SUCCESS,
  UPDATE_TRAINING_STATUS,
  CHANGE_DEFAULT_GROUP_SUCCESS,
  CHANGE_DEFAULT_GROUP_FAILED
} from './ActionType';

const intitialState = {
  personGroups: [],
  isLoading: false,
  error: ''
}

export default function commonReducer(state = intitialState, action: any) {
  switch (action.type) {
    case GET_ALL_PERSONGROUP_REQUEST: 
    case ADD_PERSONGROUP_REQUEST:
    case DELETE_PERSONGROUP_REQUEST:
    case ADD_PERSON_REQUEST:
      return Object.assign({}, state, {
        isLoading: true
      });
    case GET_ALL_PERSONGROUP_FAILED:
    case ADD_PERSONGROUP_FAILED:
    case DELETE_PERSONGROUP_FAILED:
    case ADD_PERSON_FAILED:
      return Object.assign({}, state, {
        isLoading: false,
        error: action.error
      });
    case GET_ALL_PERSONGROUP_SUCCESS: 
      return Object.assign({}, state , {
        isLoading: false,
        personGroups: action.response
      });

    case ADD_PERSONGROUP_SUCCESS:
      return Object.assign({}, state, {
        isLoading: false,
        personGroups: [...state.personGroups, action.newPersonGroup]
      });
    case DELETE_PERSONGROUP_SUCCESS:
      return Object.assign({}, state, {
        isLoading: false,
        personGroups: state.personGroups.filter(pg => pg.id !== action.delPersonGroupId)
      })
    case ADD_PERSON_SUCCESS:
      const personGroups = state.personGroups.map(pg => {
        if (pg.id === action.newPerson.personGroupId) {
          return {
            ...pg,
            persons: [...pg.persons, action.newPerson]
          }
        }
        return pg
      })
      return Object.assign({}, state, {
        isLoading: false,
        personGroups
      })
    case DELETE_PERSON_SUCCESS:
      const delPersonGroup = state.personGroups.map(pg => {
        const idxPs = pg.persons.findIndex(ps => ps.id === action.delPersonId[0])  
        if (idxPs !== -1) {
          const unchangePerson = pg.persons.filter(ps => ps.id !== action.delPersonId[0]);
          return Object.assign({}, pg, {
            persons: unchangePerson
          })
        }
        return pg
      })
      return Object.assign({}, state, {
        isLoading: false,
        personGroups: delPersonGroup
      })
    case UPDATE_TRAINING_STATUS:
      const newPersonGroups = state.personGroups.map(pg => {
        if (pg.id === action.personGroupId) {
          return {
            ...pg,
            trainingStatus: action.newStatus
          }
        }
        return pg
      })
      return Object.assign({}, state, {
        personGroups: newPersonGroups
      })
    case CHANGE_DEFAULT_GROUP_SUCCESS:
      const allPersonGroups = state.personGroups.map(pg => {
        if (pg.id === action.personGroupId) {
          return {
            ...pg,
            isDefault: true
          }
        }
        return {
          ...pg,
          isDefault: false
        }
      })
      return {
        ...state,
        personGroups: allPersonGroups
      }
    case CHANGE_DEFAULT_GROUP_FAILED:
      return {
        ...state,
        error: action.error
      }
    default:
      return state;
  }
}