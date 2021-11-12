import * as React from 'react';
import * as Redux from 'redux';
import 'semantic-ui-css/semantic.min.css';
import 'primereact/resources/themes/nova-light/theme.css';
import 'primereact/resources/primereact.min.css';
import 'primeicons/primeicons.css';

import TopBar from './Component/TopBar/TopBar';
import SideBar from './Container/SideBar/SideBar';
import { match } from 'react-router';
import * as signalR from '@aspnet/signalr';
import './App.css';
import { signalrInfoUrl, TrainingStatusName } from './Utils/Consts';
import { Growl } from 'primereact/growl';
import { connect } from 'react-redux';
import { PersonGroupEntity } from './ServiceProxies';
import { updateTrainingStatus } from './Common/Action';
import { clearGrowl } from './Growl/Action';
import * as _ from 'lodash';

export interface IAppProps {
  match?: match;
  growl: any;
  personGroups: PersonGroupEntity[];
  updateTrainingStatus: Redux.ActionCreator<any>;
  clearGrowl: Redux.ActionCreator<any>;
}

export interface IAppState {
  isShowPanel: boolean;
}

export class App extends React.Component<IAppProps, IAppState> {
  private growl: Growl;
  constructor(props: IAppProps) {
    super(props);
    this.state = {
      isShowPanel: false
    }
  }

  public componentDidMount() {
    this.setupSignalR();
  }

  public componentWillReceiveProps(nextProps: IAppProps) {
    if (!_.isEqual(nextProps.growl.messages, this.props.growl.messages) && nextProps.growl.messages) {
      if (nextProps.growl.messages.length > 0) {
        this.growl.show(nextProps.growl.messages);
        this.props.clearGrowl();
      }
    }
  }

  public render() {
    return (
      <div>
        <TopBar clickPanel={this.clickPanel} />
        <Growl ref={(el) => this.growl = el} />
        <SideBar isShowPanel={this.state.isShowPanel} 
          onHidePanel={this.onHidePanel} 
          match={this.props.match} 
        />
      </div>
    );
  }

  private setupSignalR = () => {

    fetch(signalrInfoUrl).then(responseInfo => responseInfo.json()).then(data => {
      const options = {
        accessTokenFactory: () => data.accessToken
      };
      const connection = new signalR.HubConnectionBuilder()
      .withUrl(data.url, options)
      .build();
  
      connection.on("fetchNewStatus", ((response) => {
        // tslint:disable-next-line:no-console
        console.log(response.personGroupId, response.status)
        this.props.updateTrainingStatus(response.personGroupId, response.status);
        if ( TrainingStatusName[response.status] === 'Succeeded' ) {
          const changedPersonGroup = this.props.personGroups.filter(pg => pg.id === response.personGroupId)[0];
          this.growl.show({ 
            severity: 'success', 
            summary: 'Training Status', 
            detail: `Training person group ${changedPersonGroup.name} has been succeeded`})
        }
        else if (TrainingStatusName[response.status] === 'Failed') {
          const changedPersonGroup = this.props.personGroups.filter(pg => pg.id === response.personGroupId)[0];
          this.growl.show({ 
            severity: 'error', 
            summary: 'Training Status', 
            detail: `Training person group ${changedPersonGroup.name} has been failed`})
        }
      }).bind(this))

      // tslint:disable-next-line:no-console
      connection.start().then(() => console.log("connection start"));
    }) 


  }

  private clickPanel = () => {
    this.setState({
      isShowPanel: this.state.isShowPanel === true ? false : true
    })
  }

  private onHidePanel = () => {
    this.setState({
      isShowPanel: false
    })
  }
}

export default connect(
  (state: any, ownProps: any) => ({
    growl: state.growl,
    personGroups: state.common.personGroups
  }),
  (dispatch: any, ownProps: any) => ({
    updateTrainingStatus: (personGroupId: number, newStatus: number) => 
      dispatch(updateTrainingStatus(personGroupId, newStatus)),
    clearGrowl: () => dispatch(clearGrowl())
  })
)(App);
