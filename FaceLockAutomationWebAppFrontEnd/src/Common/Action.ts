import { 
  GET_ALL_PERSONGROUP_REQUEST, 
  GET_ALL_PERSONGROUP_SUCCESS, 
  GET_ALL_PERSONGROUP_FAILED, 
  ADD_PERSONGROUP_REQUEST,
  ADD_PERSONGROUP_SUCCESS,
  ADD_PERSONGROUP_FAILED,
  DELETE_PERSONGROUP_REQUEST,
  DELETE_PERSONGROUP_SUCCESS,
  DELETE_PERSONGROUP_FAILED,
  ADD_PERSON_REQUEST,
  ADD_PERSON_SUCCESS,
  ADD_PERSON_FAILED,
  DELETE_PERSON_REQUEST,
  DELETE_PERSON_SUCCESS,
  DELETE_PERSON_FAILED,
  UPDATE_TRAINING_STATUS,
  CHANGE_DEFAULT_GROUP_REQUEST,
  CHANGE_DEFAULT_GROUP_SUCCESS,
  CHANGE_DEFAULT_GROUP_FAILED
} from './ActionType';
import { ofType } from 'redux-observable';
import { mergeMap, catchError } from 'rxjs/operators';
import { from, of } from 'rxjs';
import { PersonGroupClient, PersonGroupEntity, PersonEntity, PersonClient, PersonDTO, FaceClient } from '../ServiceProxies';


export const getAllPersonGroupEpic = (action$, state$) => 
  action$.pipe(
    ofType(GET_ALL_PERSONGROUP_REQUEST),
    mergeMap((action: any) => {
      const { resolve = null, reject = null } = action.meta;
      return from(new PersonGroupClient().getAll())
        .pipe(
          mergeMap(( result: any ) => {
            if (resolve) {
              resolve(result);
            }
            return of(getAllPersonGroupSuccess(result))
          }),
          catchError(error => { 
            if (reject) {
              reject(error)
            }
            return of(getAllPersonGroupFailed(error))
          })
      )})
  )

export const addPersonGroupEpic = (action$, state$) => 
  action$.pipe(
    ofType(ADD_PERSONGROUP_REQUEST),
    mergeMap((action: any) => {
      const { resolve = null, reject = null } = action.meta;
      return from(new PersonGroupClient().post(action.personGroupName))
        .pipe(
          mergeMap(result => { 
            if (resolve) {
              resolve(result)
            }
            return of(addPersonGroupSuccess(result))
          }),
          catchError(error => { 
            if (reject) {
              reject(error)
            }
            return of(addPersonGroupFailed(error))
          })
      )
    })
  )


export const deletePersonGroupEpic = (action$, state$) => 
  action$.pipe(
    ofType(DELETE_PERSONGROUP_REQUEST),
    mergeMap((action: any) => {
      const { resolve = null, reject = null } = action.meta;
      return from(new PersonGroupClient().delete(action.personGroupId))
      .pipe(
        mergeMap(result => {
          if (resolve) {
            resolve(result);
          }
          return of(deletePersonGroupSuccess(result))
        }),
        catchError(error => {
          if (reject) {
            reject(error);
          }
          return of(deletePersonGroupFailed(error))
        })
      )
    })
  )

export const addPersonEpic = (action$, state$) =>
  action$.pipe(
    ofType(ADD_PERSON_REQUEST),
    mergeMap((action: any) => {
      const { resolve = null, reject = null } = action.meta;
      return from(new PersonClient().post(new PersonDTO ({
        personGroupId: action.personGroupId,
        firstName: action.personFirstName,
        lastName: action.personLastName
      })))
        .pipe(
          mergeMap(result => { 
            if (resolve) {
              resolve(result);
            }
            return of(addPersonSuccess(result))
          }),
          catchError(error => { 
            if (reject) {
              reject(error);
            }
            return of(addPersonFailed(error))
          })
    )})
  )

export const deletePersonEpic = (action$, state$) =>
  action$.pipe(
    ofType(DELETE_PERSON_REQUEST),
    mergeMap((action: any) => {
      const { resolve = null, reject = null } = action.meta;
      return from(new PersonClient().delete(
        action.personGroupId,
        action.delPersonId
        ))
        .pipe(
          mergeMap(result => { 
            if (resolve) {
              resolve(result);
            }
            return of(deletePersonSuccess(result))
          }),
          catchError(error => {
            if (reject) {
              reject(error);
            }
            return of(deletePersonFailed(error))
          })
        )
    })
  )

export const changeDefaultGroupEpic = (action$, state$) =>
  action$.pipe(
    ofType(CHANGE_DEFAULT_GROUP_REQUEST),
    mergeMap((action: any) => {
      const { resolve = null, reject = null } = action.meta;
      return from(new PersonGroupClient().changeDefaultGroup(
        action.personGroupId
        ))
        .pipe(
          mergeMap(result => { 
            if (resolve) {
              resolve(result);
            }
            return of(changeDefaultGroupSuccess(result))
          }),
          catchError(error => {
            if (reject) {
              reject(error);
            }
            return of(changeDefaultPersonFailed(error))
          })
        )
    })
  )



export function getAllPersonGroupRequest(meta: any = {}) {
  return {
    type: GET_ALL_PERSONGROUP_REQUEST,
    meta
  }
}

function getAllPersonGroupSuccess(response: any) {
  return {
    type: GET_ALL_PERSONGROUP_SUCCESS,
    response
  }
}

function getAllPersonGroupFailed(error: any) {
  return {
    type: GET_ALL_PERSONGROUP_FAILED,
    error
  }
}

export function addPersonGroupRequest(personGroupName: string, meta: any = {}) {
  return {
    type: ADD_PERSONGROUP_REQUEST,
    personGroupName,
    meta
  }
}

function addPersonGroupSuccess(newPersonGroup: PersonGroupEntity) {
  return {
    type: ADD_PERSONGROUP_SUCCESS,
    newPersonGroup
  }
}

function addPersonGroupFailed(error: any) {
  return {
    type: ADD_PERSONGROUP_FAILED,
    error
  }
}

export function deletePersonGroupRequest(personGroupId: number, meta: any = {}) {
  return {
    type: DELETE_PERSONGROUP_REQUEST,
    personGroupId,
    meta
  }
}

function deletePersonGroupSuccess(delPersonGroupId: number) {
  return {
    type: DELETE_PERSONGROUP_SUCCESS,
    delPersonGroupId
  }
}

function deletePersonGroupFailed(error: any) {
  return {
    type: DELETE_PERSONGROUP_FAILED,
    error
  }
}

export function addPersonRequest(personGroupId: number, personFirstName: string, 
  personLastName: string, meta: any = {}) {
  return {
    type: ADD_PERSON_REQUEST,
    personGroupId,
    personFirstName,
    personLastName,
    meta
  }
}

function addPersonSuccess(newPerson: PersonEntity) {
  return {
    type: ADD_PERSON_SUCCESS,
    newPerson
  }
}

function addPersonFailed(error: any) {
  return {
    type: ADD_PERSON_FAILED,
    error
  }
}

export function deletePersonRequest(personGroupId: number, delPersonId: number[], meta: any = {}) {
  return {
    type: DELETE_PERSON_REQUEST,
    personGroupId,
    delPersonId,
    meta
  }
}

function deletePersonSuccess(delPersonId: number[]) {
  return {
    type: DELETE_PERSON_SUCCESS,
    delPersonId
  }
}

function deletePersonFailed(error: any) {
  return {
    type: DELETE_PERSON_FAILED,
    error
  }
}

export function updateTrainingStatus(personGroupId: number, newStatus: number) {
  return {
    type: UPDATE_TRAINING_STATUS,
    personGroupId,
    newStatus
  }
}

export function changeDefaultGroupRequest(personGroupId: number, meta: any = {}) {
  return {
    type: CHANGE_DEFAULT_GROUP_REQUEST,
    personGroupId,
    meta
  }
}

function changeDefaultGroupSuccess(personGroupId: number) {
  return {
    type: CHANGE_DEFAULT_GROUP_SUCCESS,
    personGroupId
  }
}

function changeDefaultPersonFailed(error: any) {
  return {
    type: CHANGE_DEFAULT_GROUP_FAILED,
    error
  }
}