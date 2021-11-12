import * as React from 'react';
import * as Redux from 'redux';
import { InputText } from 'primereact/inputtext';
import { addingTrainingType, dropdown } from 'src/Utils/Consts';
import { DropzoneComponent } from 'react-dropzone-component';
import { Sidebar, Form, Menu, Container, Button, Header, Dropdown, DropdownItemProps, Loader, Segment } from 'semantic-ui-react';
import { Label } from 'semantic-ui-react';
import 'react-dropzone-component/styles/filepicker.css';
import 'dropzone/dist/min/dropzone.min.css';
import { connect } from 'react-redux';
import { PersonGroupEntity } from 'src/ServiceProxies';
import { addPersonGroupRequest, deletePersonGroupRequest, addPersonRequest, deletePersonRequest } from 'src/Common/Action';
import { addFacesRequest } from './Actions';
import { showGrowl } from 'src/Growl/Action';
import { showSuccessMessage, showErrorMessage } from 'src/Utils/GrowlMessage';


interface IAddProps {
  addingType: number;
  isShowPanel: boolean;
  onClosePanel: () => void;
  personGroups?: PersonGroupEntity[];
  addPersonGroup?: Redux.ActionCreator<any>;
  addPerson?: Redux.ActionCreator<any>;
  addFaces?: Redux.ActionCreator<any>;
  isSavingPerson?: boolean;
  isSavingFace?: boolean;
  showGrowl?: Redux.ActionCreator<any>;
}

interface IAddState {
  personGroupName?: string,
  personFirstName?: string,
  personLastName?: string,
  uploadFaces?: File[],
  selectedPersonGroupId?: number,
  selectedPersonId?: number,
}

class Add extends React.Component<IAddProps, IAddState> {

  constructor(props: IAddProps) {
    super(props);
    const defaultPersonGroup = props.personGroups.filter(pg => pg.isDefault)[0];
    const defaultPersons = defaultPersonGroup.persons;

    this.state = {
      personGroupName: '',
      personFirstName: '',
      personLastName: '',
      uploadFaces: [],
      selectedPersonGroupId: defaultPersonGroup.id,
      selectedPersonId: defaultPersons ? defaultPersons[0].id : null,
    }
  }

  public render(): JSX.Element {
    const { isShowPanel } = this.props;

    return (
      <Sidebar visible={isShowPanel}
        as={Menu}
        animation='overlay'
        direction='right'
        width='very wide'
        vertical
      >
        {this.renderForm()}
      </Sidebar>
    )
  }

  private renderForm(): JSX.Element {
    const { addingType, personGroups, isSavingPerson, isSavingFace } = this.props;
    const { selectedPersonGroupId } = this.state;

    const personGroupOpts = personGroups.map(pg => ({
      key: pg.id,
      value: pg.id,
      text: pg.name
    }));

    const persons = personGroups.filter(pg => pg.id === selectedPersonGroupId)[0].persons;
    const personOpts = persons ? persons.map(ps => ({
      key: ps.id,
      value: ps.id,
      text: `${ps.firstName} ${ps.lastName}`
    })) : [];

    const actionBtn = (
      <React.Fragment>
        <Segment loading={isSavingPerson || isSavingFace}>
          <Button onClick={this.onSave}>Save</Button>
          <Button onClick={this.onHidePanel}>Cancel</Button>
        </Segment>
      </React.Fragment>
    )

    let formElement = null;
    if (addingType === addingTrainingType.personGroup) {
      formElement = this.renderAddPersonGroupForm(actionBtn);
    } else if (addingType === addingTrainingType.person) {
      formElement = this.renderAddPersonForm(actionBtn, personGroupOpts);
    } else {
      formElement = this.renderAddFaceForm(actionBtn, personGroupOpts, personOpts);
    }

    return formElement;
  }

  private renderAddPersonGroupForm(actionBtn: JSX.Element): JSX.Element {
    return (
      <Menu.Item>
        <Header as='h3' icon='group' content='Add Person Group' />
        <Form>
          <Form.Field>
            <Label>Person Group Name</Label>
            <InputText value={this.state.personGroupName} onChange={this.onChangePersonGroupName} />
          </Form.Field>
          {actionBtn}
        </Form>
      </Menu.Item>
    )
  }

  private renderAddPersonForm(actionBtn: JSX.Element, personGroupOpts: DropdownItemProps[]): JSX.Element {
    return (
      <Menu.Item>
        <Header as='h3' icon='user' content='Add Person' />
        <Form>
          <Form.Field>
            <Label>Person Group</Label>
            <Dropdown placeholder="Person Group" search selection options={personGroupOpts}
              value={this.state.selectedPersonGroupId}
              onChange={this.onChangePersonGroupId}
            />
          </Form.Field>
          <Form.Field>
            <Label>Person First Name</Label>
            <InputText value={this.state.personFirstName} onChange={this.onChangePersonFirstName} />
          </Form.Field>
          <Form.Field>
            <Label>Person Last Name</Label>
            <InputText value={this.state.personLastName} onChange={this.onChangePersonLastName} />
          </Form.Field>
          {actionBtn}
        </Form>
      </Menu.Item>
    )
  }

  private renderAddFaceForm(actionBtn: JSX.Element, personGroupOpts: DropdownItemProps[],
    personOpts: DropdownItemProps[]): JSX.Element {
    const componentConfig = {
      iconFiletypes: ['.jpg', '.png'],
      showFiletypeIcon: true,
      postUrl: 'no-url'

    };
    const djsConfig = {
      autoProcessQueue: false,
      uploadMultiple: true,
    }

    return (
      <Menu.Item>
        <Header as='h3' icon='image' content='Add Faces' />
        <Form>
          <Form.Field>
            <Label>Person Group</Label>
            <Dropdown placeholder="Person Group" search selection options={personGroupOpts}
              value={this.state.selectedPersonGroupId}
              onChange={this.onChangePersonGroupId}
            />
            <Form.Field>
              <Label>Person</Label>
              <Dropdown placeholder="Person" search selection options={personOpts}
                value={this.state.selectedPersonId}
                onChange={this.onChangePersonId}
              />
            </Form.Field>
          </Form.Field>
          <Form.Field>
            <Label>Upload face</Label>
            <DropzoneComponent
              config={componentConfig}
              djsConfig={djsConfig}
              eventHandlers={{ addedfile: (files) => this.onUploadFaces(files) }}
            />
          </Form.Field>
          {actionBtn}
        </Form>
      </Menu.Item>
    )
  }

  private onChangePersonGroupName = (event: any) => {
    this.setState({
      personGroupName: event.target.value
    })
  }

  private onChangePersonFirstName = (event: any) => {
    this.setState({
      personFirstName: event.target.value
    })
  }

  private onChangePersonLastName = (event: any) => {
    this.setState({
      personLastName: event.target.value
    })
  }

  private onChangePersonGroupId = (event: React.SyntheticEvent, data: any) => {
    if (data.value === this.state.selectedPersonGroupId) {
      return;
    }
    this.setState({
      selectedPersonGroupId: data.value
    })
  }

  private onChangePersonId = (event: React.SyntheticEvent, data: any) => {
    if (data.value === this.state.selectedPersonId) {
      return;
    }
    this.setState({
      selectedPersonId: data.value
    })
  }

  private onUploadFaces = (file) => {
    const { uploadFaces } = this.state;

    this.setState({
      uploadFaces: uploadFaces.length ? [...uploadFaces, file] : [ file ]
    })

  }

  private resetPanel = () => {
    const defaultPersonGroup = this.props.personGroups.filter(pg => pg.isDefault)[0];
    const defaultPersons = defaultPersonGroup.persons;
    this.setState({
      personGroupName: '',
      personFirstName: '',
      personLastName: '',
      selectedPersonGroupId: defaultPersonGroup.id,
      selectedPersonId: defaultPersons ? defaultPersons[0].id : null,
      uploadFaces: []
    })
  }

  private onHidePanel = () => {
    this.resetPanel();
    this.props.onClosePanel();
  }

  private onSave = () => {
    const { addingType } = this.props;
    const { 
      selectedPersonGroupId,
      selectedPersonId,
      personGroupName,
      personFirstName,
      personLastName,
      uploadFaces
    } = this.state;

    switch (addingType) {
      case addingTrainingType.personGroup:
        if (!personGroupName.length) {
          return null;
        }
        return new Promise((resolve, reject) => 
          this.props.addPersonGroup(personGroupName, { resolve, reject }))
          .then(res => {
            showSuccessMessage(this, 'Success', 'Person group has been created!');
            this.onHidePanel();
          })
          .catch(error => {
            showErrorMessage(this, 'Failed', error);
          });
      case addingTrainingType.person:
        if (!selectedPersonGroupId || !personFirstName.length || !personLastName.length) {
          return null;
        }
        return new Promise((resolve, reject) => 
          this.props.addPerson(selectedPersonGroupId, personFirstName, personLastName, { resolve, reject}))
          .then(res => {
            showSuccessMessage(this, 'Success', 'Person has been created!');
            this.onHidePanel();
          })
          .catch(error => {
            showErrorMessage(this, 'Failed', error);
          });
      case addingTrainingType.face:
        if (!selectedPersonGroupId || !selectedPersonId || !uploadFaces.length) {
          return null;
        }
        return new Promise((resolve, reject) => 
          this.props.addFaces(selectedPersonGroupId, selectedPersonId, uploadFaces, { resolve, reject }))
          .then(res => {
            showSuccessMessage(this, 'Success', 'Faces has been created!');
            this.onHidePanel();
          })
          .catch(error => {
            showErrorMessage(this, 'Failed', error);
          });
    }
    return null;
  }
}

export default connect(
  (state: any, ownProps: any) => ({
    isSavingPerson: state.common.isLoading,
    isSavingFace: state.trainingFace.isLoading
  }),
  (dispatch: any, ownProps: any) => ({
    addPersonGroup: (personGroupName: string, meta: any) => 
      dispatch(addPersonGroupRequest(personGroupName, meta)),
    addPerson: (personGroupId: number, personFirstName: string, personLastName: string, meta: any) =>
      dispatch(addPersonRequest(personGroupId, personFirstName, personLastName, meta)),
    addFaces: (personGroupId: number, personId: number, uploadFaces: File[], meta: any) =>
      dispatch(addFacesRequest(personGroupId, personId, uploadFaces, meta)),
    showGrowl: (message: any) => dispatch(showGrowl(message))
  })
)(Add);