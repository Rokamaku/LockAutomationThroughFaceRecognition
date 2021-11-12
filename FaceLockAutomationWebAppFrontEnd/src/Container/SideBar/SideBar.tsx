import * as React from 'react';
import { Header, Icon, Image, Menu, Segment, Sidebar, Container, Label, SidebarPushable, Ref } from 'semantic-ui-react';
import ActivityLog from '../ActivityLog/ActivityLog';
import FaceTraining from '../FaceTraining/FaceTraining';
import { urlPaths } from 'src/Utils/Consts';
import { match, Route, Switch } from 'react-router';
import { Link } from 'react-router-dom';

export interface ISideBarProps {
  isShowPanel: boolean;
  match?: match;
  onHidePanel?: () => void;
}

export interface ISideBarState {
}

export default class SideBar extends React.Component<ISideBarProps, ISideBarState> {
  private segmentRef;
  constructor(props: ISideBarProps) {
    super(props);
    this.state = {
    }
  }

  public render() {
    return (
      <Sidebar.Pushable >
        { this.handleRef && (
        <Sidebar
          as={Menu}
          animation='overlay'
          icon='labeled'
          inverted
          vertical
          visible={this.props.isShowPanel}
          width='thin'
          onHide={this.handleHide}
          target={this.segmentRef}
          style={{ margin: 0 }}
        >
          <Menu.Item as={ Link } name="activityLog" to={urlPaths.ActivityLog} >
            <Icon name='clipboard list' />
            Activity Log
          </Menu.Item>
          <Menu.Item as={ Link} name="trainingFace" to={urlPaths.FaceTraining} >
            <Icon name='sign language' />
            Face Training
          </Menu.Item>
        </Sidebar>   
        )}
        <Ref innerRef={this.handleRef} >    
          <Sidebar.Pusher dimmed={this.props.isShowPanel}>
            <Switch>
              <Route exact path={urlPaths.Root} component={ActivityLog} />
              <Route path={urlPaths.ActivityLog} component={ActivityLog} />
              <Route path={urlPaths.FaceTraining} component={FaceTraining} />
            </Switch>  
          </Sidebar.Pusher>
        </Ref>
      </Sidebar.Pushable>
    )
  }

  private handleRef = (ref: any) => {
    this.segmentRef = ref;
  }

  private handleHide = () => {
    this.props.onHidePanel();
  }
}