import * as React from 'react';
import * as Redux from 'redux';
import { isEqual } from 'lodash';
import { 
  Image, 
  Item, 
  Grid, 
  Loader, 
  GridColumn, 
  Dimmer, 
  Segment, 
  Pagination, 
  Menu, 
  Dropdown, 
  Container, 
  Label, 
  Sticky, 
  Rail, 
  Ref, 
  List, 
  Modal, 
  Header, 
  Button,
  Icon
} from 'semantic-ui-react';
import { connect } from 'react-redux';
import { 
  getFaceLogInPersonGroupRequest, 
  getKnownFaceLogRequest, 
  getUnknownFaceLogRequest, 
  getUndetectedFaceLogRequest, 
  getPersonFaceLogRequest 
} from './Actions';
import { FaceLogEntity } from '../../ServiceProxies';
import { PaginationHeader, PersonGroupEntity } from 'src/ServiceProxies';
import { SyntheticEvent } from 'react';
import { FilterModel } from 'src/Utils/FilterModel';
import { getAllPersonGroupRequest } from 'src/Common/Action';
import { SingleDatePicker, isInclusivelyBeforeDay } from 'react-dates';
import 'react-dates/initialize';
import 'react-dates/lib/css/_datepicker.css';
import '../../react_dates_overrides.css';
import * as moment from 'moment';
import { dropdown, sortTypeKey, faceLogType } from 'src/Utils/Consts';
import "./ActivityLog.css";
import { TagImage } from 'src/Component/TagImage/TagImage';


export interface IActivityLogProps {
  getFaceLogInPersonGroup?: Redux.ActionCreator<any>;
  getKnownFaceLog?: Redux.ActionCreator<any>;
  getUnknownFaceLog?: Redux.ActionCreator<any>;
  getUndetectedFaceLog?: Redux.ActionCreator<any>;
  getPersonFaceLog?: Redux.ActionCreator<any>;
  getAllPersonGroup?: Redux.ActionCreator<any>;
  faceLogs?: FaceLogEntity[];
  isLoading?: boolean;
  paging?: PaginationHeader;
  personGroups?: PersonGroupEntity[];
}

export interface IActivityLogState {
  activePage?: number;
  showEllipsis?: boolean;
  showFirstAndLastNav?: boolean;
  showPreviousAndNextNav?: boolean;
  fromDate?: moment.Moment;
  toDate?: moment.Moment;
  fromDateFocused?: boolean;
  toDateFocused?: boolean;
  selectedPersonGroupId?: number;
  selectedSortType?: string;
  selectedPageSize?: number;
  selectedLogType?: number;
  selectedPersonId?: number;
  contextRef?: any;
  isShowDetail?: boolean;
  selectedModal?: FaceLogEntity;
  isFilterChange?: boolean;
}

class ActivityLog extends React.Component<IActivityLogProps, IActivityLogState> {
  constructor(props: IActivityLogProps) {
    super(props);

    const fromDate = moment().subtract(7, 'days');
    const toDate = moment();
    this.state = {
      activePage: 1,
      showEllipsis: true,
      showFirstAndLastNav: true,
      showPreviousAndNextNav: true,
      fromDate,
      toDate,
      fromDateFocused: false,
      toDateFocused: false,
      selectedPersonGroupId: dropdown.intializeNum,
      selectedSortType: sortTypeKey.descending,
      selectedPageSize: 25,
      selectedLogType: faceLogType.allFaceLog,
      selectedPersonId: dropdown.all,
      contextRef: null,
      isShowDetail: false,
      selectedModal: null,
      isFilterChange: false
    }
    
  }

  public componentDidMount() {
    this.props.getAllPersonGroup();
  }

  public componentDidUpdate(prevProps: IActivityLogProps) {
    if (!isEqual(prevProps.personGroups, this.props.personGroups) || 
      this.state.selectedPersonGroupId === dropdown.intializeNum) {
      const { personGroups } = this.props;
      if (personGroups.length !== 0) {
        const defaultPersonGroup = personGroups.filter(pg => pg.isDefault)[0];
        this.setState({
          selectedPersonGroupId: defaultPersonGroup.id
        }, this.onFiltering)
      }
    }
  }

  public render() : JSX.Element {
    const { isLoading, personGroups, paging } = this.props;
    const { 
      isShowDetail,
      activePage,
      showEllipsis,
      showFirstAndLastNav,
      showPreviousAndNextNav
    } = this.state;
    const personGroupOpts = personGroups.map(pg => ({
      key: pg.id,
      value: pg.id,
      text: pg.name,
    }))

    const currentPersonGroup = personGroups.filter(pg => pg.id === this.state.selectedPersonGroupId)[0];
    const personOpts = currentPersonGroup ? currentPersonGroup.persons.map(ps => ({
      key: ps.id,
      value: ps.id,
      text: `${ps.firstName} ${ps.lastName}`
    })) : []

    personOpts.unshift({
      key: dropdown.all,
      value: dropdown.all,
      text: 'All'
    })

    const faceLogTypeOpts = [
      {
        key: faceLogType.allFaceLog,
        value: faceLogType.allFaceLog,
        text: 'All face log'
      },
      {
        key: faceLogType.knownFaceLog,
        value: faceLogType.knownFaceLog,
        text: 'Known face log'
      },
      {
        key: faceLogType.unknownFaceLog,
        value: faceLogType.unknownFaceLog,
        text: 'Unknown face log'
      },
      {
        key: faceLogType.undetectedFaceLog,
        value: faceLogType.undetectedFaceLog,
        text: 'Undetected face log'
      }
    ]

    const sortTypeOpts = [
      {
        key: 'asc',
        value: 'asc',
        text: 'Ascending'
      },
      {
        key: 'desc',
        value: 'desc',
        text: 'Descending'
      }
    ]

    const numberRowsPerPage = [ 10, 15, 25, 50, 100 ]
    const recPerPageOpts = numberRowsPerPage.map(nr => ({
      key: nr,
      value: nr,
      text: nr
    }))

    const pageHeight = {
      height: paging.totalPages ? "calc(100vh - 310px)" : "calc(100vh - 260px)"
    }

    return (
      <Segment>
        <Menu borderless attached="top">
          <Menu.Item className="menu-item-container">
            <Label ribbon className="menu-item-label">Person Group</Label>
            <Dropdown placeholder="Person Group" search selection options={personGroupOpts} 
              className="menu-item-content"
              value={this.state.selectedPersonGroupId}
              onChange={this.onPersonGroupChange}
            />
          </Menu.Item>
          <Menu.Item className="menu-item-container">
            <Label ribbon className="menu-item-label">Person</Label>
            <Dropdown placeholder="Person" search selection options={personOpts} 
              className="menu-item-content"
              value={this.state.selectedPersonId}
              onChange={this.onPersonChange}
            />
          </Menu.Item>
          <Menu.Item className="menu-item-container">
            <Label ribbon className="menu-item-label">Log Type</Label>
            <Dropdown placeholder="Log Type" selection options={faceLogTypeOpts} 
              className="menu-item-content"
              value={this.state.selectedLogType}
              onChange={this.onLogTypeChange}
            />
          </Menu.Item>
          <Menu.Item className="menu-item-container">
            <Label ribbon className="menu-item-label">Sort Type</Label>
            <Dropdown placeholder="Sort Type" selection options={sortTypeOpts}
              className="menu-item-content"
              value={this.state.selectedSortType}
              onChange={this.onSortTypeChange}
            />
          </Menu.Item>
          <Menu.Item className="menu-item-container">
            <Label ribbon className="menu-item-label">From Date</Label>
            <div>
              <SingleDatePicker id="fromDate" 
                date={this.state.fromDate} 
                onDateChange={this.onSelectFromDateChange}
                focused={this.state.fromDateFocused}
                onFocusChange={this.onFromDateFocusChange}
                isOutsideRange={this.isOutsideRange}              
              />
            </div>
          </Menu.Item>
          <Menu.Item className="menu-item-container">
            <Label ribbon className="menu-item-label">To Date</Label>
            <div >
              <SingleDatePicker id="toDate" 
                date={this.state.toDate} 
                onDateChange={this.onSelectToDateChange}
                focused={this.state.toDateFocused}
                onFocusChange={this.onToDateFocusChange}
                isOutsideRange={this.isOutsideRange}  
              />
            </div>
          </Menu.Item>
        </Menu>
        <Segment raised>
          <Grid columns={5} >
            <Grid.Row centered divided>
              <Grid.Column floated="left">
                <Button content="Refresh" icon="refresh" 
                labelPosition="left" 
                onClick={this.onFiltering}
                />
              </Grid.Column>
              <Grid.Column floated="left">
                <Dropdown placeholder="Page size" selection options={recPerPageOpts} icon="expand"
                  value={this.state.selectedPageSize}
                  onChange={this.onPageSizeChange}
                />
              </Grid.Column>
            </Grid.Row>
          </Grid>
          <Segment loading={isLoading} placeholder className="item-log-container" style={pageHeight}>
            { !isLoading ? this.renderGrid() : null}
          </Segment>
          { paging.totalPages ? (
          <Grid textAlign="center" >
            <Grid.Row centered>
              <Pagination 
                activePage={activePage}
                totalPages={paging.totalPages} 
                onPageChange={this.onPaginationChange}
                boundaryRange={2}
                siblingRange={2} 
                ellipsisItem={showEllipsis ? undefined : null}
                firstItem={showFirstAndLastNav ? undefined : null}
                lastItem={showFirstAndLastNav ? undefined : null}
                prevItem={showPreviousAndNextNav ? undefined : null}
                nextItem={showPreviousAndNextNav ? undefined : null}
                pointing
                secondary
              />
            </Grid.Row>
          </Grid>
          ) : null}
        </Segment>
        { isShowDetail ? this.renderModal() : null }
      </Segment>
    )
  }

  private renderModal() : JSX.Element {
    const { isShowDetail, selectedModal} = this.state;
    const { personGroups } = this.props;
    return (
      <Modal open={isShowDetail} closeIcon size="large" onClose={this.onModalClose}>
        <Modal.Header>
          Log Detail
        </Modal.Header>
        <Modal.Content image>
          <TagImage faceLog={selectedModal} persons={personGroups[0].persons} />
          <Modal.Description style={{ padding: '10px'}}>
            <Header>{selectedModal.file.fileName}</Header>
            <p>Created Date: {selectedModal.createdDate.toLocaleString()}</p>
          </Modal.Description>
        </Modal.Content>
      </Modal>
    )
  }

  private renderGrid(): JSX.Element {
    const { faceLogs, paging } = this.props;
    const rowSize = 5;

    let itemCells = [];
    for (let idxRow = 0; idxRow < faceLogs.length; idxRow += rowSize) {
      for (let idxCol = 0; idxRow + idxCol < faceLogs.length && idxCol < rowSize; idxCol++) {
        const currentIdx = idxRow + idxCol;
        const newItemCol = (
          <Grid.Column key={faceLogs[currentIdx].id} className="item-log-column">
            <List.Item value={currentIdx.toString()} onClick={this.onItemClick} className="log-item">
              <Image src={faceLogs[currentIdx].file.uri} size="small" wrapped/>
            </List.Item>
          </Grid.Column>
        )
        itemCells = itemCells.length > 0 ? [...itemCells, newItemCol] : [ newItemCol ]
      }
    }
    const grid = (
      <Grid.Row>
        {itemCells}
      </Grid.Row>
    )

    return (
      <React.Fragment>
        { paging.totalPages ? (
          <Grid columns={rowSize} textAlign="center" >
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

  private onFiltering = () => {
    if (!this.state.fromDate || !this.state.toDate) {
      return;
    }
    const filter = new FilterModel();
    filter.pageSize = this.state.selectedPageSize;
    filter.pageNumber = !this.state.isFilterChange ? this.state.activePage : 1;
    filter.fromDate = this.state.fromDate.toDate();
    filter.toDate = this.state.toDate.toDate();
    filter.sortType = this.state.selectedSortType;

    this.setState({
      isFilterChange: false
    })

    if (this.state.selectedPersonId !== dropdown.all) {
      this.props.getPersonFaceLog(this.state.selectedPersonGroupId, this.state.selectedPersonId, filter);
      return;
    }

    switch (this.state.selectedLogType) {
      case faceLogType.allFaceLog:
        this.props.getFaceLogInPersonGroup(this.state.selectedPersonGroupId, filter);
        return;
      case faceLogType.knownFaceLog:
        this.props.getKnownFaceLog(this.state.selectedPersonGroupId, filter);
        return;
      case faceLogType.unknownFaceLog:
        this.props.getUnknownFaceLog(this.state.selectedPersonGroupId, filter);
        return;
      case faceLogType.undetectedFaceLog:
        this.props.getUndetectedFaceLog(this.state.selectedPersonGroupId, filter);
        return;
      default:
        this.props.getFaceLogInPersonGroup(this.state.selectedPersonGroupId, filter);
    }
  }

  private onItemClick = (event: any, data: any) => {
    const { faceLogs } = this.props;
    this.setState({
      isShowDetail: true,
      selectedModal: faceLogs[data.value]
    })
  }

  private onModalClose = (event, data: any) => {
    this.setState({
      isShowDetail: false
    })
  }

  private onPersonGroupChange = (event: SyntheticEvent, data: any) => {
    if (data.value === this.state.selectedPersonGroupId) {
      return;
    }
    this.setState({
      selectedPersonGroupId: data.value,
      isFilterChange: true
    }, this.onFiltering)
  }

  private onPersonChange = (event: SyntheticEvent, data: any) => {
    if (data.value === this.state.selectedPersonId) {
      return;
    }
    if (data.value !== dropdown.all) {
      this.setState({
        selectedPersonId: data.value,
        selectedLogType: faceLogType.allFaceLog,
        isFilterChange: true
      }, this.onFiltering)
    }
    else {
      this.setState({
        selectedPersonId: data.value,
        isFilterChange: true
      }, this.onFiltering)
    }
  }

  private onLogTypeChange = (event: SyntheticEvent, data: any) => {
    if (data.value === this.state.selectedLogType) {
      return;
    }
    if (data.value !== faceLogType.allFaceLog && this.state.selectedPersonId !== dropdown.all) {
      this.setState({
        selectedLogType: data.value,
        selectedPersonId: dropdown.all,
        isFilterChange: true
      }, this.onFiltering)
    }
    this.setState({
      selectedLogType: data.value,
      isFilterChange: true
    }, this.onFiltering)
  }

  private onSortTypeChange = (event: SyntheticEvent, data: any) => {
    if (data.value === this.state.selectedSortType) {
      return;
    }
    this.setState({
      selectedSortType: data.value,
      isFilterChange: true
    }, this.onFiltering)
  }

  private onPageSizeChange = (event: SyntheticEvent, data: any) => {
    if (data.value === this.state.selectedPageSize) {
      return;
    }
    this.setState({
      selectedPageSize: data.value,
      isFilterChange: true
    }, this.onFiltering)
  }

  private onFromDateFocusChange = ({ focused }) => {
    this.setState({
      fromDateFocused: focused
    })
  } 

  private onToDateFocusChange = ({ focused }) => {
    this.setState({
      toDateFocused: focused
    })
  } 

  private onSelectFromDateChange = (date) => {
    this.setState({ 
      fromDate: date,
      isFilterChange: true
    }, this.onFiltering)
  }

  private onSelectToDateChange = (date) => {
    this.setState({ 
      toDate: date,
      isFilterChange: true
    }, this.onFiltering)
  }

  private isOutsideRange = (day) => {
    return !isInclusivelyBeforeDay(day, moment())
  }

  private onPaginationChange = (event: SyntheticEvent, data: any) => {
    this.setState({
      activePage: !this.state.isFilterChange ? data.activePage : 1,
    }, this.onFiltering)
  }
}

export default connect(
  (state: any, ownProps: any) => ({
    isLoading: state.activityLog.isLoading,
    faceLogs: state.activityLog.faceLogs,
    paging: state.activityLog.paging,
    personGroups: state.common.personGroups
  }), 
  (dispatch: any, ownProps: any) => ({
    getFaceLogInPersonGroup: (personGroupId: number, filter: FilterModel) => 
      dispatch(getFaceLogInPersonGroupRequest(personGroupId, filter)),
    getKnownFaceLog: (personGroupId: number, filter: FilterModel) => 
      dispatch(getKnownFaceLogRequest(personGroupId, filter)),
    getUnknownFaceLog: (personGroupId: number, filter: FilterModel) => 
      dispatch(getUnknownFaceLogRequest(personGroupId, filter)),
    getUndetectedFaceLog: (personGroupId: number, filter: FilterModel) => 
      dispatch(getUndetectedFaceLogRequest(personGroupId, filter)),
    getPersonFaceLog: (personGroupId: number, personId: number, filter: FilterModel) => 
      dispatch(getPersonFaceLogRequest(personGroupId, personId, filter)),
    getAllPersonGroup: (meta: any) => dispatch(getAllPersonGroupRequest(meta))
}))(ActivityLog);