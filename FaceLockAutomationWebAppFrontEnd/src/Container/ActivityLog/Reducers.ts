import { GET_FACELOG_IN_GROUP_REQUEST, GET_FACELOG_SUCCESS, GET_FACELOG_FAILED, GET_KNOWN_FACELOG_REQUEST, GET_UNKNOWN_FACELOG_REQUEST, GET_UNDETECTED_FACELOG_REQUEST, GET_PERSON_FACELOG_REQUEST } from './ActionTypes';

const initialState = {
  faceLogs: [],
  isLoading: false,
  error: '',
  paging: {
    pageNumber: 1,
    pageSize: 15,
    totalCount: 0,
    totalPages: 0,
  }
};


export default function ActivityLogReducer(state = initialState, action: any) {
  switch (action.type) {
    case GET_KNOWN_FACELOG_REQUEST:
    case GET_UNKNOWN_FACELOG_REQUEST:
    case GET_UNDETECTED_FACELOG_REQUEST:
    case GET_PERSON_FACELOG_REQUEST:
    case GET_FACELOG_IN_GROUP_REQUEST:
      return {
        ...state,
        isLoading: true,
      }
    case GET_FACELOG_SUCCESS:
      return {
        ...state,
        isLoading: false,
        faceLogs: action.response.data,
        paging: action.response.paginationHeader
      }
    case GET_FACELOG_FAILED:
      return {
        ...state,
        isLoading: false,
        error: action.error
      }
    default:
      return state;
  }
}