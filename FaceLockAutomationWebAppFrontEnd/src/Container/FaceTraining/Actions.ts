import { 
  GET_PERSONFACE_REQUEST, 
  GET_PERSONFACE_SUCCESS, 
  GET_PERSONFACE_FAILED,
  ADD_FACE_REQUEST,
  ADD_FACE_SUCCESS,
  ADD_FACE_FAILED,
  DELETE_FACE_REQUEST,
  DELETE_FACE_SUCCESS,
  DELETE_FACE_FAILED
} from './ActionTypes';
import { ofType } from 'redux-observable';
import { mergeMap, catchError } from 'rxjs/operators';
import { FaceClient, FaceEntity } from 'src/ServiceProxies';
import { from, of } from 'rxjs';


export const getPersonFaceEpic = (action$, state$) =>
  action$.pipe(
    ofType(GET_PERSONFACE_REQUEST),
    mergeMap((action: any) => {
      const { resolve = null, reject = null } = action.meta;
      return from(new FaceClient().getAll(action.personGroupId, action.personId))
        .pipe(
          mergeMap(result => { 
            if (resolve) {
              resolve(result);
            }
            return of(getPersonFaceSuccess(result))
          }),
          catchError(error => { 
            if (reject) {
              reject(error);
            }
            return of(getPersonFaceFailed(error))
          })
        )
    })
  )

export const addFaceEpic = (action$, state$) =>
  action$.pipe(
    ofType(ADD_FACE_REQUEST),
    mergeMap((action: any) => {
      const { resolve = null, reject = null } = action.meta; 
      return from(new FaceClient().post(
        action.personGroupId,
        action.personId,
        action.faces
      ))
      .pipe(
        mergeMap(result => { 
          if (resolve) {
            resolve(result);
          }
          return of(addFacesSuccess(result))
        }),
        catchError(error => { 
          if (reject) {
            reject(error);
          }
          return of(addFaceFailed(error))
        })
      )
    })
  )

export const deleteFaceEpic = (action$, state$) => 
  action$.pipe(
    ofType(DELETE_FACE_REQUEST),
    mergeMap((action: any) => {
      const { resolve = null, reject = null } = action.meta;
      return from(new FaceClient().delete(
        action.personGroupId,
        action.personId,
        [ action.faceId ]
      ))
      .pipe(
        mergeMap(result => { 
          if (resolve) {
            resolve(result);
          }
          return of(deleteFaceSuccess(result[0]))
        }),
        catchError(error => { 
          if (reject) {
            reject(error);
          }
          return of(deleteFaceFailed(error))
        })
      )
    })
  )


export function getPersonFaceRequest(personGroupId: number, personId: number, meta: any = {}) {
  return {
    type: GET_PERSONFACE_REQUEST,
    personGroupId,
    personId,
    meta
  }
}

function getPersonFaceSuccess(response: any) {
  return {
    type: GET_PERSONFACE_SUCCESS,
    response
  }
}

function getPersonFaceFailed(error: any) {
  return {
    type: GET_PERSONFACE_FAILED,
    error
  }
}

export function addFacesRequest(personGroupId: number, personId: number, faces: File[], meta: any = {}) {
  return {
    type: ADD_FACE_REQUEST,
    personGroupId,
    personId,
    faces,
    meta
  }
}

function addFacesSuccess(faces: FaceEntity[]) {
  return {
    type: ADD_FACE_SUCCESS,
    faces
  }
}

function addFaceFailed(error: any) {
  return {
    type: ADD_FACE_FAILED,
    error
  }
}

export function deleteFaceRequest(personGroupId: number, personId: number, faceId: number, meta: any = {}) {
  return {
    type: DELETE_FACE_REQUEST,
    personGroupId,
    personId,
    faceId,
    meta
  }
}

function deleteFaceSuccess(delFaceId: number) {
  return {
    type: DELETE_FACE_SUCCESS,
    delFaceId
  }
}

function deleteFaceFailed(error: any) {
  return {
    type: DELETE_FACE_FAILED,
    error
  }
}

