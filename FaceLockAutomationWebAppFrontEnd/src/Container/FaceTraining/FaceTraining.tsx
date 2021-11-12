import * as React from 'react';
import * as Redux from 'redux';
import { 
  Image, 
  Segment, 
  Button, 
  Grid, 
  List, 
  Container, 
  Sidebar, 
  Modal, 
  Header, 
  Icon, 
  Label 
} from 'semantic-ui-react';
import { connect } from 'react-redux';
import { getPersonFaceRequest, deleteFaceRequest } from './Actions';
import { FaceEntity, PersonGroupEntity, TrainingClient } from 'src/ServiceProxies';
import { getAllPersonGroupRequest, deletePersonGroupRequest, deletePersonRequest, changeDefaultGroupRequest } from 'src/Common/Action';
import { Tree } from 'primereact/tree';
import { Menubar } from 'primereact/menubar';
import Add from './Add';
import { addingTrainingType, TrainingStatusName } from 'src/Utils/Consts';
import './FaceTraining.css';
import { showSuccessMessage, showErrorMessage } from 'src/Utils/GrowlMessage';
import { showGrowl } from 'src/Growl/Action';



export interface IFaceTrainingProps {
  faces?: FaceEntity[];
  isFacesLoading?: boolean;
  personGroups?: PersonGroupEntity[];
  isPersonGroupsLoading?: boolean;
  getPersonFace?: Redux.ActionCreator<any>;
  getAllPersonGroup?: Redux.ActionCreator<any>;
  deletePersonGroup?: Redux.ActionCreator<any>;
  deletePerson?: Redux.ActionCreator<any>;
  deleteFace?: Redux.ActionCreator<any>;
  changeDefaultGroup?: Redux.ActionCreator<any>;
  showGrowl?: Redux.ActionCreator<any>;
}

export interface IFaceTrainingState {
  selectedPersonGroupId?: number;
  selectedPersonId?: number;
  selectedFaceId?: number;
  expandedKeys?: any;
  showAddPanel?: boolean;
  addingType?: number;
  showDeleteModal?: boolean;
}

class FaceTraining extends React.Component<IFaceTrainingProps, IFaceTrainingState> {
  constructor(props: IFaceTrainingProps) {
    super(props);
    this.state = {
      selectedPersonGroupId: null,
      selectedPersonId: null,
      selectedFaceId: null,
      showAddPanel: false,
      addingType: null,
      showDeleteModal: false
    }
  }

  public componentDidMount() {
    this.props.getAllPersonGroup();
  }

  public render(): JSX.Element {
    const { isFacesLoading, isPersonGroupsLoading, personGroups } = this.props;
    const { 
      showAddPanel, 
      addingType, 
      showDeleteModal, 
      selectedPersonGroupId,
      selectedPersonId,
      selectedFaceId 
    } = this.state;

    const contentheight = {
      height: "calc(100vh - 160px)"
    }

    const currentSelectedPersonGroup = selectedPersonGroupId ? 
      personGroups.filter(pg => pg.id === selectedPersonGroupId)[0] : null;
    const currentPersonGroupStatus = currentSelectedPersonGroup ? 
      TrainingStatusName[currentSelectedPersonGroup.trainingStatus] : null;
    const currentDefaultGroup = currentSelectedPersonGroup ?
      currentSelectedPersonGroup.isDefault ? 'Yes' : 'No' : null;

    const menuItems = [
      {
        label: 'Add',
        icon: 'pi pi-plus',
        disabled: personGroups.length === 0,
        items: [
          {
            label: 'Person Group',
            icon: 'pi pi-users',
            command: this.onClickAddPersonGroup
          },
          {
            label: 'Person',
            icon: 'pi pi-user',
            command: this.onClickAddPerson
          },
          {
            label: 'Faces',
            icon: 'pi pi-image',
            command: this.onClickAddFaces
          }
        ]
      },
      {
        label: 'Delete',
        icon: 'pi pi-minus',
        disabled: !selectedPersonGroupId && !selectedPersonId && !selectedFaceId,
        command: this.onShowDeletePanel
      },
      {
        label: 'Refresh',
        icon: 'pi pi-refresh',
        disabled: !selectedPersonGroupId || !selectedPersonId,
        command: this.onRefresh
      },
      {
        label: 'Make default',
        icon: 'pi pi-lock-open',
        disabled: !currentSelectedPersonGroup || currentSelectedPersonGroup.isDefault,
        command: this.onChangeDefaultGroup
      },
      {
        label: 'Train',
        icon: 'pi pi-cloud-upload',
        disabled: !selectedPersonGroupId,
        command: this.onTraining
      }
    ];

    return (
      <Sidebar.Pushable as={Segment}>
        <Sidebar.Pusher dimmed={showAddPanel}>
          <Segment>
            <Menubar model={menuItems} >
              { selectedPersonGroupId && [
                <Label key="default" color="blue">
                  { `Default: ${currentDefaultGroup}`}
                </Label>,
                <Label key="status" color="blue">
                  { `Status: ${currentPersonGroupStatus}` }
                </Label>
              ]
              }
            </Menubar>
            <Segment>
              <Grid columns="2">
                <Grid.Column width="4" >
                  {!isPersonGroupsLoading ? this.renderList() : null}
                </Grid.Column>
                <Grid.Column width="12" >
                  <Segment loading={isFacesLoading} placeholder style={contentheight} className="face-container">
                    {!isFacesLoading ? this.renderFace() : null}
                  </Segment>
                </Grid.Column>
              </Grid>
            </Segment>
          </Segment>
          {showDeleteModal && this.renderModal()}
        </Sidebar.Pusher>
        {personGroups.length !== 0 && (
          <Add
            addingType={addingType}
            isShowPanel={showAddPanel}
            onClosePanel={this.onCloseAddPanel}
            personGroups={personGroups}
          />
        )}
      </Sidebar.Pushable>
    )
  }

  private renderModal(): JSX.Element {
    const { personGroups, faces } = this.props;
    const { showDeleteModal, selectedPersonGroupId, selectedPersonId, selectedFaceId } = this.state;
    const isDeleteFace = selectedPersonGroupId != null && selectedPersonId != null && selectedFaceId != null;
    const isDeletePerson = selectedPersonGroupId != null && selectedPersonId != null && selectedFaceId == null;
    const isDeletePersonGroup = selectedPersonGroupId != null && selectedPersonId == null && selectedFaceId == null;
    const selectedPersonGroup = personGroups.filter(pg => pg.id === selectedPersonGroupId)[0];
    const selectedPerson = selectedPersonGroup.persons.filter(ps => ps.id === selectedPersonId)[0];
    const selectedFace = isDeleteFace ? faces.filter(fc => fc.id === selectedFaceId)[0] : null;
    return (
      <Modal open={showDeleteModal} basic size='small'>
        <Header icon='delete' content='Delete' />
        <Modal.Content>
          <p>
            Are you sure you want to delete this
              { isDeletePersonGroup ? ` person group '${selectedPersonGroup.name}'` :
                isDeletePerson ? ` person '${selectedPerson.firstName} ${selectedPerson.lastName}'` : 
                isDeleteFace ? ` face '${selectedFace.file.fileName}' of person '${selectedPerson.firstName} ${selectedPerson.lastName}'` : null }
          </p>
        </Modal.Content>
        <Modal.Actions>
          <Button basic color='red' icon inverted onClick={this.onCloseDeletePanel}>
            <Icon name='remove'/>No
          </Button>
          <Button color='green' icon inverted onClick={this.onDelete}>
            <Icon name='checkmark' />Yes
          </Button>
        </Modal.Actions>
      </Modal>
    )
  }

  private renderList(): JSX.Element {
    const { personGroups, isPersonGroupsLoading } = this.props;

    const personGroupData = personGroups.map(pg => ({
      key: [pg.id],
      label: pg.name,
      icon: 'pi pi-users',
      children: pg.persons ? pg.persons.map(ps => ({
        key: [pg.id, ps.id],
        label: `${ps.firstName} ${ps.lastName}`,
        icon: 'pi pi-user',
        children: []
      })) : []
    }))

    return (
      <React.Fragment>
        {personGroups ? (
          <Tree value={personGroupData}
            style={{ height: 'calc(100vh - 160px)' }}
            selectionMode="single"
            onSelectionChange={this.onTreeNodeSelectChange}
            loading={isPersonGroupsLoading} 
            />
        ) : null}
      </React.Fragment>
    )
  }


  private renderFace(): JSX.Element {
    const { faces } = this.props;
    const rowSize = 5;

    let itemCells = [];
    for (let idxRow = 0; idxRow < faces.length; idxRow += rowSize) {
      for (let idxCol = 0; idxRow + idxCol < faces.length && idxCol < rowSize; idxCol++) {
        const currentIdx = idxRow + idxCol;
        const newItemCol = (
          <Grid.Column key={faces[currentIdx].id} className="face-column" >
            <List.Item value={currentIdx.toString()} onClick={this.onFaceSelected} className="face-item">
              <Image src={faces[currentIdx].file.uri} size="small" wrapped />
            </List.Item>
          </Grid.Column>
        )
        itemCells = itemCells.length > 0 ? [...itemCells, newItemCol] : [newItemCol]
      }
    }
    const grid = (
      <Grid.Row>
        {itemCells}
      </Grid.Row>
    )

    return (
      <React.Fragment>
        { faces.length ? (
          <Grid columns={rowSize} textAlign="center">
            {grid}
          </Grid>
        ) : (
            <Container textAlign="center">
              <Image src="https://cdn.dribbble.com/users/634336/screenshots/2246883/_____.png"
                className="empty-image"
                size="medium"
              />
            </Container>
          )}
      </React.Fragment>
    )
  }

  private onChangeDefaultGroup = () => {
    this.props.changeDefaultGroup(this.state.selectedPersonGroupId);
  }

  private onTreeNodeSelectChange = (e: { originalEvent: Event, value: any[] }): void => {
    if (e.value.length < 2) {
      this.setState({
        selectedPersonGroupId: e.value[0],
        selectedPersonId: null,
      })
      return;
    }
    this.props.getPersonFace(e.value[0], e.value[1]);
    this.setState({
      selectedPersonGroupId: e.value[0],
      selectedPersonId: e.value[1]
    })
  }


  private onFaceSelected = (event: any, data: any) => {
    this.setState({
      selectedFaceId: this.props.faces[data.value].id
    })
  }

  private onClickAddPersonGroup = () => {
    this.setState({
      addingType: addingTrainingType.personGroup,
      showAddPanel: true
    })
  }

  private onClickAddPerson = () => {
    this.setState({
      addingType: addingTrainingType.person,
      showAddPanel: true
    })
  }

  private onClickAddFaces = () => {
    this.setState({
      addingType: addingTrainingType.face,
      showAddPanel: true
    })
  }

  private onShowDeletePanel = () => {
    this.setState({
      showDeleteModal: true
    })
  }

  private onDelete = () => {
    const { selectedPersonGroupId, selectedPersonId, selectedFaceId } = this.state;
    this.onCloseDeletePanel();
    if (selectedPersonGroupId && selectedPersonId && selectedFaceId) {
      return new Promise((resolve, reject) =>        
        this.props.deleteFace(selectedPersonGroupId, selectedPersonId, selectedFaceId, { resolve, reject }))
        .then(result => { 
          showSuccessMessage(this, 'Success', 'Face has been deleted!');
        })
        .catch(error => { 
          showErrorMessage(this, 'Failed', error);
        });

    }
    else if (selectedPersonGroupId && selectedPersonId) {
      return new Promise((resolve, reject) => 
        this.props.deletePerson(selectedPersonGroupId, [selectedPersonId], { resolve, reject }))
        .then(result => {
          showSuccessMessage(this, 'Success', 'Person has been deleted');
        })
        .catch(error => {
          showErrorMessage(this, 'Failed', error);
        });
    }
    else if (selectedPersonGroupId) {
      return new Promise((resolve, reject) => 
        this.props.deletePersonGroup(selectedPersonGroupId, { resolve, reject }))
        .then(result => {
          showSuccessMessage(this, 'Success', 'Person group has been deleted');
        })
        .catch(error => {
          showErrorMessage(this, 'Failed', error);
        });
    }

    return null;
  }

  private onRefresh = () => {
    this.props.getPersonFace(this.state.selectedPersonGroupId, this.state.selectedPersonId);
  }

  private onTraining = () => {
    new TrainingClient().post(this.state.selectedPersonGroupId);
  }

  private onCloseDeletePanel = () => {
    this.setState({
      showDeleteModal: false
    })
  }

  private onCloseAddPanel = () => {
    this.setState({
      showAddPanel: false,
    })
  }
}

export default connect(
  (state: any) => ({
    faces: state.trainingFace.faces,
    isFacesLoading: state.trainingFace.isLoading,
    personGroups: state.common.personGroups,
    isPersonGroupsLoading: state.common.isLoading
  }),
  (dispatch: any) => ({
    getPersonFace: (personGroupId: number, personId: number, meta: any) =>
      dispatch(getPersonFaceRequest(personGroupId, personId, meta)),
    getAllPersonGroup: (meta: any) => dispatch(getAllPersonGroupRequest(meta)),
    deletePersonGroup: (personGroupId: number, meta: any) =>
      dispatch(deletePersonGroupRequest(personGroupId, meta)),
    deletePerson: (personGroupId: number, delPersonId: number[], meta: any) =>
      dispatch(deletePersonRequest(personGroupId, delPersonId, meta)),
    deleteFace: (personGroupId: number, personId: number, faceId: number, meta: any) => 
      dispatch(deleteFaceRequest(personGroupId, personId, faceId, meta)),
    changeDefaultGroup: (personGroupId: number, meta: any) => 
      dispatch(changeDefaultGroupRequest(personGroupId, meta)),
    showGrowl: (message: any) => dispatch(showGrowl(message))
  })
)(FaceTraining);
