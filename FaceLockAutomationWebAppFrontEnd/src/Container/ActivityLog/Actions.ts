import { mergeMap, catchError} from 'rxjs/operators';
import { ofType } from 'redux-observable';
import { GET_FACELOG_IN_GROUP_REQUEST, 
  GET_FACELOG_SUCCESS, 
  GET_FACELOG_FAILED, 
  GET_PERSON_FACELOG_REQUEST,
  GET_KNOWN_FACELOG_REQUEST,
  GET_UNKNOWN_FACELOG_REQUEST,
  GET_UNDETECTED_FACELOG_REQUEST,
} from './ActionTypes';
import { FaceLogClient } from '../../ServiceProxies';
import { of, from } from 'rxjs';
import { FilterModel } from 'src/Utils/FilterModel';

export const getFaceLogInGroupEpic = (action$) => 
  action$.pipe(
    ofType(GET_FACELOG_IN_GROUP_REQUEST),
    mergeMap((action: any) => 
      from(new FaceLogClient().getFaceLogInPersonGroup(
        action.personGroupId, 
        action.filter.pageSize, 
        action.filter.pageNumber,
        action.filter.sortType,
        action.filter.fromDate,
        action.filter.toDate))
        .pipe(
          mergeMap(result => of(getFaceLogSuccess(result))),
          catchError(error => of(getFaceLogFailed(error)))
        )
    )
  )

export const getPersonFaceLogEpic = (action$) => 
  action$.pipe(
    ofType(GET_PERSON_FACELOG_REQUEST),
    mergeMap((action: any) => 
      from(new FaceLogClient().getPersonFaceLog(
        action.personGroupId, 
        action.personId,
        action.filter.pageSize,
        action.filter.pageNumber,
        action.filter.sortType,
        action.filter.fromDate,
        action.filter.toDate))
        .pipe(
          mergeMap(result => of(getFaceLogSuccess(result))),
          catchError(err => of(getFaceLogFailed(err)))
        )
    ),
  )

export const getKnownFaceLogEpic = (action$) => 
  action$.pipe(
    ofType(GET_KNOWN_FACELOG_REQUEST),
    mergeMap((action: any) => 
      from(new FaceLogClient().getKnownFaceLogInGroup(
        action.personGroupId, 
        action.filter.pageSize, 
        action.filter.pageNumber,
        action.filter.sortType,
        action.filter.fromDate,
        action.filter.toDate))
        .pipe(
          mergeMap(result => of(getFaceLogSuccess(result))),
          catchError(error => of(getFaceLogFailed(error)))
        )
    )
  )

export const getUnknownFaceLogEpic = (action$) => 
  action$.pipe(
    ofType(GET_UNKNOWN_FACELOG_REQUEST),
    mergeMap((action: any) => 
      from(new FaceLogClient().getUnknownFaceLogInGroup(
        action.personGroupId, 
        action.filter.pageSize, 
        action.filter.pageNumber,
        action.filter.sortType,
        action.filter.fromDate,
        action.filter.toDate))
        .pipe(
          mergeMap(result => of(getFaceLogSuccess(result))),
          catchError(error => of(getFaceLogFailed(error)))
        )
    )
  )

export const getUndetectedFaceLogEpic = (action$) => 
  action$.pipe(
    ofType(GET_UNDETECTED_FACELOG_REQUEST),
    mergeMap((action: any) => 
      from(new FaceLogClient().getUndetectedFaceLogInGroup(
        action.personGroupId, 
        action.filter.pageSize, 
        action.filter.pageNumber,
        action.filter.sortType,
        action.filter.fromDate,
        action.filter.toDate))
        .pipe(
          mergeMap(result => of(getFaceLogSuccess(result))),
          catchError(error => of(getFaceLogFailed(error)))
        )
    )
  )

export function getFaceLogInPersonGroupRequest(personGroupId: number, filter: FilterModel ) {
  return {
    type: GET_FACELOG_IN_GROUP_REQUEST,
    personGroupId,
    filter
  }
}

export function getPersonFaceLogRequest(personGroupId: number, personId: number, filter: FilterModel) {
  return {
    type: GET_PERSON_FACELOG_REQUEST,
    personGroupId,
    personId,
    filter
  }
}

export function getKnownFaceLogRequest(personGroupId: number, filter: FilterModel ) {
  return {
    type: GET_KNOWN_FACELOG_REQUEST,
    personGroupId,
    filter
  }
}

export function getUnknownFaceLogRequest(personGroupId: number, filter: FilterModel) {
  return {
    type: GET_UNKNOWN_FACELOG_REQUEST,
    personGroupId,
    filter
  }
}

export function getUndetectedFaceLogRequest(personGroupId: number, filter: FilterModel) {
  return {
    type: GET_UNDETECTED_FACELOG_REQUEST,
    personGroupId,
    filter
  }
}

function getFaceLogSuccess(response: any) {
  return {
    type: GET_FACELOG_SUCCESS,
    response
  }
}

function getFaceLogFailed(error: any) {
  return {
    type: GET_FACELOG_FAILED,
    error
  }
}


